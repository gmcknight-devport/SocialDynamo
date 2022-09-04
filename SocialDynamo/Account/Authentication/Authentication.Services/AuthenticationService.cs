using Account.API.Commands;
using Account.API.Common.ViewModels;
using Account.API.Infrastructure.Repositories;
using Account.API.ViewModels;
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
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IAuthenticationRepository _authenticationRepo;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration,
                                    TokenValidationParameters tokenValidationParameters,
                                    IAuthenticationRepository authenticationRepo,
                                    IUserRepository userRepository,
                                    ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _authenticationRepo = authenticationRepo;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task HandleCommandAsync(RegisterUserCommand command)
        {
            var newUser = new User
            {
                EmailAddress = command.EmailAddress,
                Password = command.Password,
                Forename = command.Forename,
                Surname = command.Surname
            };

            await _userRepository.AddUser(newUser);
        }

        public async Task HandleCommandAsync(LogoutUserCommand command)
        {
            await _authenticationRepo.RemoveToken(command.UserId);
        }

        public async Task<IActionResult> HandleCommandAsync(LoginUserCommand command)
        {
            User user = await _userRepository.GetUserAsync(command.EmailAddress);
            //User user = await _userManager.FindByEmailAsync(command.EmailAddress);

            if (user != null)
            {
                if (await _authenticationRepo.AuthenticateUser(user.UserId, command.Password))
                //if(await _userManager.CheckPasswordAsync(user, command.Password))
                {                    
                    //Generate tokens
                    var token = GenerateToken(user.UserId);

                    var refreshToken = GenerateRefreshToken();
                    DateTime refreshExpiresAt = DateTime.Today.AddDays(1);
                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                    //Assign to user and save
                    await _authenticationRepo.UpdateToken(user.UserId, refreshToken, refreshExpiresAt);

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

        private JwtSecurityToken GenerateToken(int userId)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
            var expiresAt = DateTime.UtcNow.AddMinutes(20);
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
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

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal;
            
            principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))

                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
