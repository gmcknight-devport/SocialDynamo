using Account.API.Commands;
using Account.API.Common.ViewModels;
using Account.Domain.Repositories;
using Account.API.ViewModels;
using Account.Exceptions;
using Account.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Account.API.Services
{
    //All services implemented for authentication in the project.
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationRepository _authenticationRepo;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration,
                                    IAuthenticationRepository authenticationRepo,
                                    IUserRepository userRepository,
                                    ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _authenticationRepo = authenticationRepo;
            _userRepository = userRepository;
            _logger = logger;
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
                Followers = new List<Follower>()
            };

            await _userRepository.AddUserAsync(newUser);
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
            User user = await _userRepository.GetUserAsync(command.EmailAddress);
            //User user = await _userManager.FindByEmailAsync(command.EmailAddress);

            if (user != null)
            {
                if (await _authenticationRepo.AuthenticateUser(user.UserId, HashPassword(command.Password)))
                //if(await _userManager.CheckPasswordAsync(user, command.Password))
                {                    
                    //Generate tokens
                    var token = GenerateToken(user.UserId);

                    var refreshToken = GenerateRefreshToken();
                    DateTime refreshExpiresAt = DateTime.Today.AddDays(1);
                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                    //Assign to user and save
                    await _authenticationRepo.UpdateToken(user.UserId, refreshToken, refreshExpiresAt);

                    _logger.LogInformation("----- User authenticated, generated JWT token. " +
                        "User: {@EmailAdress}", command.EmailAddress);

                    //Create result to return
                    return new OkObjectResult(new AuthResultVM
                    {
                        Token = jwtToken,
                        RefreshToken = refreshToken,
                        ExpiresAt = token.ValidTo
                    });
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
        /// Handles the RefreshJWTTokenCommand. Generates a JWT access token after 
        /// validating the currently held refresh token.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<IActionResult> HandleCommandAsync(RefreshJwtTokenCommand command)
        {
            //Check claim principal from expired access token
            try
            {
                var principal = GetPrincipalFromExpiredToken(command.Token);
            }catch (ArgumentNullException)
            {
                return new BadRequestObjectResult("Invalid request");
            }

            //Check validity of tokens against stored refresh token - the proper security measure
            var refToken = await _authenticationRepo.GetRefreshToken(command.UserId);            
            OkObjectResult? refreshResult = refToken as OkObjectResult;
            Tuple<string, DateTime>? refreshTuple = refreshResult.Value as Tuple<string, DateTime>;
            
            RefreshTokenVM userRefreshToken = new RefreshTokenVM
            {
                RefreshToken = refreshTuple.Item1,
                TokenExpires = refreshTuple.Item2
            };

            if (userRefreshToken == null ||
                userRefreshToken.RefreshToken.ToString() != command.RefreshToken ||
                userRefreshToken.TokenExpires <= DateTime.Now)
            {               
                return new BadRequestObjectResult("Invalid refresh token");
            }

            _logger.LogInformation("----- User refresh token validated, generating new access token. " +
                        "User: {@UserId}", command.UserId);

            // Generate new tokens
            var accessToken = GenerateToken(command.UserId);
            var refreshToken = GenerateRefreshToken();
            DateTime refreshExpiresAt = DateTime.Today.AddDays(1);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

            //Update user token
            await _authenticationRepo.UpdateToken(command.UserId, refreshToken, refreshExpiresAt);

            //Return tokens
            return new OkObjectResult(new AuthResultVM
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                ExpiresAt = refreshExpiresAt
            });
        }

        /// <summary>
        /// Private method to ash the password for secure storage. 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var asBytes = Encoding.Default.GetBytes(password);
            var hashed = sha.ComputeHash(asBytes);

            return Convert.ToBase64String(hashed);
        }

        /// <summary>
        /// Returns validation parameters to ensure the token conforms.
        /// </summary>
        /// <returns></returns>
        private TokenValidationParameters GetTokenValidationParameters()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = securityKey
            };

            return tokenValidationParameters;
        }

        /// <summary>
        /// Generates a new token for the passed in user. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private JwtSecurityToken GenerateToken(string userId)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
            var expiresAt = DateTime.UtcNow.AddMinutes(20);
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Create Security Token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
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
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
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
    }
}
