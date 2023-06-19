using Account.API.Commands;
using Account.Domain.ViewModels;
using Account.Domain.Repositories;
using Account.Exceptions;
using Account.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using System.Net.Mime;
using Account.API.ViewModels;
using Account.API.IntegrationEvents;
using Common.OptionsConfig;
using Microsoft.Extensions.Options;

namespace Account.API.Services
{
    //All services implemented for authentication in the project.
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepo;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationService> _logger;

        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly string _jwtSecret;
        private readonly string _serviceBus;

        public AuthenticationService(IConfiguration baseConfiguration,
                                     IOptions<JwtOptions> jwtOptions,
                                     IOptions<ConnectionOptions> connectionOptions,
                                     IAuthenticationRepository authenticationRepo,
                                     IUserRepository userRepository,
                                     ILogger<AuthenticationService> logger)
        {
            _authenticationRepo = authenticationRepo;
            _userRepository = userRepository;
            _logger = logger;

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
            if (!_userRepository.IsUserIdUnique(command.UserId).Result)
                throw new InvalidUserStateException("UserId is not unique");

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
        public async Task<IActionResult> HandleCommandAsync(LoginUserCommand command)
        {
            User user = await _userRepository.GetUserByEmailAsync(command.EmailAddress);

            if (user != null)
            {
                if (await _authenticationRepo.AuthenticateUser(user.UserId, HashPassword(command.Password)))
                {
                    _logger.LogInformation("----- User authenticated, generated JWT token. " +
                        "User: {@EmailAdress}", command.EmailAddress);

                    return await GenerateTokens(user.UserId, null);
                }
                else
                {
                    return new UnauthorizedObjectResult("Password is incorrect");
                }
            }
            else
            {
                return new BadRequestObjectResult("No account for this email address");
            }
        }
        
        /// <summary>
        /// Handles the RefreshJwtTokenCommand. Checks validity of the token, checks
        /// refresh token expiry and calls methods to create and return new tokens. 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<IActionResult> HandleCommandAsync(RefreshJwtTokenCommand command)
        {
            var userRefreshToken = await _authenticationRepo.GetRefreshToken(command.UserId);

            //Check claim principal from expired access token
            try
            {
                var principal = GetPrincipalFromExpiredToken(command.Token);

                return await GenerateTokens(command.UserId, userRefreshToken);
                
            }catch(SecurityTokenExpiredException){
                if (userRefreshToken.RefreshExpires >= DateTime.UtcNow)
                {
                    return await GenerateTokens(command.UserId, userRefreshToken);
                }
                else if (userRefreshToken != null ||
                        userRefreshToken.RefreshToken.ToString() == command.RefreshToken ||
                        userRefreshToken.RefreshExpires >= DateTime.UtcNow)
                {                    
                    return await GenerateTokens(command.UserId, null);                    
                }
                return new BadRequestObjectResult("Invalid request");
            }
        }

        /// <summary>
        /// Generates new tokens based on parameter values. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private async Task<IActionResult> GenerateTokens(string userId, RefreshTokenVM refreshToken)
        {
            var accessToken = GenerateToken(userId);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

            if (refreshToken == null)
            {
                refreshToken = new()
                {
                    RefreshToken = GenerateRefreshToken(),
                    RefreshExpires = DateTime.UtcNow.AddDays(1)
                };                
                                
                await _authenticationRepo.UpdateToken(userId, refreshToken.RefreshToken, refreshToken.RefreshExpires);
            }

            _logger.LogInformation("----- User refresh token validated, generating new access token. " +
                "User: {@UserId}", userId);

            return new OkObjectResult(new AuthResultVM
            {
                Token = jwtToken,
                RefreshToken = refreshToken.RefreshToken,
                ExpiresAt = refreshToken.RefreshExpires
            });
        }
              
        /// <summary>
        /// Generates a new token for the passed in user. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private JwtSecurityToken GenerateToken(string userId)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret));
            var expiresAt = DateTime.UtcNow.AddMinutes(20);
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
        /// Gets the security principal from the expired token passed in. 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var _tokenValidationParameters = GetTokenValidationParameters();
            ClaimsPrincipal principal;
            
            principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))

                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        /// <summary>
        /// Returns validation parameters to ensure the token conforms.
        /// </summary>
        /// <returns></returns>
        private TokenValidationParameters GetTokenValidationParameters()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret));
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtIssuer,
                ValidAudience = _jwtAudience,
                IssuerSigningKey = securityKey
            };

            return tokenValidationParameters;
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
