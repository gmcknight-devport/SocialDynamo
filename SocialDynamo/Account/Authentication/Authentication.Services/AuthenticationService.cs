using Common.API.Commands;
using Common.Domain.ViewModels;
using Common.Domain.Repositories;
using Common.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common.Exceptions;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using System.Net.Mime;
using Common.API.ViewModels;
using Common.API.IntegrationEvents;
using Common.OptionsConfig;
using Microsoft.Extensions.Options;

namespace Common.API.Services
{
    //All services implemented for authentication in the project.
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepo;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly string _jwtSecret;
        private readonly string _serviceBus;

        public AuthenticationService(IConfiguration baseConfiguration,
                                     IOptions<JwtOptions> jwtOptions,
                                     IOptions<ConnectionOptions> connectionOptions,
                                     IAuthenticationRepository authenticationRepo,
                                     IUserRepository userRepository,
                                     ILogger<AuthenticationService> logger, 
                                     IHttpContextAccessor httpContextAccessor)
        {
            _authenticationRepo = authenticationRepo;
            _userRepository = userRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            if (baseConfiguration["JwtIssuer"] != null)
            {
                _jwtIssuer = baseConfiguration["JwtIssuer"];
                _jwtAudience = baseConfiguration["JwtAudience"];
                _jwtSecret = baseConfiguration["JwtSecret"];
                _serviceBus = baseConfiguration["ServiceBus"];
            }
            else
            {
                _jwtIssuer = jwtOptions.Value.JwtIssuer;
                _jwtAudience = jwtOptions.Value.JwtAudience;
                _jwtSecret = jwtOptions.Value.JwtSecret;
                _serviceBus = connectionOptions.Value.ServiceBus;
            }
        }

        /// <summary>
        /// Handles the register user command.
        /// Registeres a new user based on the information provided 
        /// calling the appropriate repository.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="InvalidUserStateException"></exception>
        public async Task HandleCommandAsync(RegisterUserCommand command)
        {
            if (!_userRepository.IsUserIdUnique(command.UserId))
                throw new InvalidUserStateException("UserId is not unique");

            if (!_userRepository.IsEmailUnique(command.EmailAddress))
                throw new InvalidUserStateException("Email is not unique");

            _logger.LogInformation("----- Registering new user");

            var newUser = new User
            {
                EmailAddress = command.EmailAddress,
                UserId = command.UserId,
                Password = HashPassword(command.Password),
                Forename = command.Forename,
                Surname = command.Surname,
                ProfileDescription = "",
                RefreshToken = "",
                RefreshExpires = DateTime.UtcNow.AddDays(1),
                Followers = new List<Follower>()
            };
                        
            await _userRepository.AddUserAsync(newUser);

            NewUserIntegrationEvent integrationEvent = new()
            {
                UserId = command.UserId
            };

            PublishEvent(integrationEvent);
        }

        /// <summary>
        /// Handles the LogoutUserCommand. Logs out user by removing the JWT token.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task HandleCommandAsync(LogoutUserCommand command)
        {
            _logger.LogInformation("----- Removing token");
            await _authenticationRepo.RemoveToken(command.UserId);
        }

        /// <summary>
        /// Handles the LoginUserCommand logging the user in if the email address 
        /// and password entered are valid. Generates a token to allow user to 
        /// remain signed in and access the rest of the application.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<IActionResult> HandleCommandAsync(LoginUserCommand command, HttpContext httpContext)
        {
            try 
            {
                User user = await _userRepository.GetUserByEmailAsync(command.EmailAddress);

                if (await _authenticationRepo.AuthenticateUser(user.UserId, HashPassword(command.Password)))
                    _logger.LogInformation("----- User authenticated, generated JWT token. " +
                        "User: {@EmailAdress}", command.EmailAddress);
                else         
                    throw new ArgumentException("Password is incorrect");

                return await GenerateTokens(user.UserId, null, httpContext);

            }
            catch(ArgumentNullException ex)
            {                
                throw new ArgumentNullException("No account for this email address");
            }
        }

        /// <summary>
        /// Handles the RefreshJwtTokenCommand. Checks validity of the token, checks
        /// refresh token expiry and calls methods to create and return new tokens. 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<IActionResult> HandleCommandAsync(RefreshJwtTokenCommand command, HttpContext context)
        {
            var userRefreshToken = await _authenticationRepo.GetRefreshToken(command.UserId);

            if (userRefreshToken.RefreshExpires >= DateTime.UtcNow)
            {
                return await GenerateTokens(command.UserId, userRefreshToken, context);
            }
       
            //Remove token from user and return BadRequest
            await _authenticationRepo.UpdateToken(command.UserId, null, DateTime.UtcNow.AddDays(-1));
            throw new ArgumentException("Invalid request - token expired");
        }

        /// <summary>
        /// Generates new tokens based on parameter values. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private async Task<IActionResult> GenerateTokens(string userId, RefreshTokenVM refreshToken, HttpContext httpContext)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(0.5);
            var cookieOptions = new CookieOptions
            {
                Expires = expiresAt,
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            var accessToken = GenerateToken(userId, expiresAt);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

            //Set token as cookie
            httpContext.Response.Cookies.Append("token", jwtToken, cookieOptions);
            
            //Generate new refresh token
            refreshToken = new()
            {
                RefreshToken = GenerateRefreshToken(),
                RefreshExpires = DateTime.UtcNow.AddDays(1)
            };                
                                
            await _authenticationRepo.UpdateToken(userId, refreshToken.RefreshToken, refreshToken.RefreshExpires);
            
            _logger.LogInformation("----- User refresh token validated, User: {@UserId}", userId);
                        
            return new OkObjectResult(new AuthResultVM
            {
                UserId = userId
            });
        }
              
        /// <summary>
        /// Generates a new token for the passed in user. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private JwtSecurityToken GenerateToken(string userId, DateTime expiresAt)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret));
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Create Security Token
            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                expires: expiresAt,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return token;
        }

        /// <summary>
        /// Generates a new refresh token. 
        /// </summary>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return BitConverter.ToString(randomNumber);
        }
                
        /// <summary>
        /// Private method to ash the password for secure storage. 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var asBytes = Encoding.Default.GetBytes(password);
            var hashed = sha.ComputeHash(asBytes);

            return Convert.ToBase64String(hashed);
        }

        /// <summary>
        /// Publishes integration event passed into it. 
        /// </summary>
        /// <param name="integrationEvent"></param>
        private async void PublishEvent(IIntegrationEvent integrationEvent)
        {
            var jsonMessage = JsonConvert.SerializeObject(integrationEvent);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            var client = new ServiceBusClient(_serviceBus);
            var sender = client.CreateSender(integrationEvent.GetType().Name);

            var message = new ServiceBusMessage()
            {
                Body = new BinaryData(body),
                MessageId = Guid.NewGuid().ToString(),
                ContentType = MediaTypeNames.Application.Json,
                Subject = integrationEvent.GetType().Name
            };

            await sender.SendMessageAsync(message);
            _logger.LogInformation("----- New NewUserIntegrationEvent created and send. " +
                "MessageId: {@MessageId}, Body: {@Body}", message.MessageId, message.Body);
        }
    }
}
