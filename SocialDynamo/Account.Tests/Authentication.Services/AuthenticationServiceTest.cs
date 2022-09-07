using Account.API.Commands;
using Account.API.Infrastructure.Repositories;
using Account.API.Services;
using Account.API.ViewModels;
using Account.Models.Users;
using Autofac.Extras.Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Account.Tests.Authentication.Services
{
    public class AuthenticationServiceTest
    {
        private readonly ITestOutputHelper _output;

        public AuthenticationServiceTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task HandleCommand_RegisterUser()
        {
            RegisterUserCommand command = new RegisterUserCommand()
            {
                EmailAddress = "goo@goo.com",
                Password = "password",
                Forename = "Forefore",
                Surname = "Sursur"
            };

            User user = new()
            {
                EmailAddress = command.EmailAddress,
                Password = command.Password,
                Forename = command.Forename,
                Surname = command.Surname
            };

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IUserRepository>()
                    .Setup(x => x.AddUserAsync(It.IsAny<User>()))
                    .Verifiable();

                //Create instance of class and call method
                var testClass = mock.Create<AuthenticationService>();
                await testClass.HandleCommandAsync(command);

                //Verify method on mock was called once
                mock.Mock<IUserRepository>()                    
                    .Verify(x => x.AddUserAsync(It.Is<User>(u => u.UserId == user.UserId &&
                                                               u.Password == user.Password &&
                                                               u.Forename == user.Forename &&
                                                               u.Surname == user.Surname)),
                                                               Times.Exactly(1));
            }
        }

        [Fact]
        public async Task HandleCommandAsync_LogoutUser()
        {
            LogoutUserCommand command = new LogoutUserCommand()
            {
                UserId = 1
            };

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IAuthenticationRepository>()
                    .Setup(x => x.RemoveToken(command.UserId))
                    .Verifiable();

                //Create instance of class and call method
                var testClass = mock.Create<AuthenticationService>();
                await testClass.HandleCommandAsync(command);

                //Verify method on mock was called once
                mock.Mock<IAuthenticationRepository>()
                    .Verify(x => x.RemoveToken(command.UserId), Times.Exactly(1));
            }
        }
        [Fact]
        public async Task HandleCommand_LoginUser()
        {
            User user = GetSampleUser();
            LoginUserCommand command = GetLoginCommand();

            Random rnd = new Random();
            byte[] b = new byte[16];
            rnd.NextBytes(b);

            string secret = Convert.ToBase64String(b);

            var myConfiguration = new Dictionary<string, string>
                {
                    {"JWT:Issuer", "Me"},
                    {"JWT:Audience", "OtherPerson"},
                    {"JWT:Secret", secret}
                };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var mock = new Mock<IUserRepository>();
            mock.Setup(x => x.GetUserAsync(command.EmailAddress))
                .Returns(Task.FromResult(user));
            
            var authMock = new Mock<IAuthenticationRepository>();
            authMock.Setup(x => x.AuthenticateUser(user.UserId, user.Password))
                    .Returns(Task.FromResult(true));

            var testClass = new AuthenticationService(null, configuration, null, authMock.Object, mock.Object, null);

            var expected = new OkObjectResult(new AuthResultVM());

            OkObjectResult actual = (OkObjectResult)await testClass.HandleCommandAsync(command);

            Assert.Equal(expected.Value.GetType(), actual.Value.GetType());
        }

        [Fact]
        public async Task HandleCommand_LoginUser_UnauthorisedException()
        {
            User user = GetSampleUser();
            LoginUserCommand command = GetLoginCommand();

            using (var mock = AutoMock.GetLoose())
            {
                //Mock
                mock.Mock<IUserRepository>()
                    .Setup(x => x.GetUserAsync(command.EmailAddress))
                    .Returns(Task.FromResult(user));

                mock.Mock<IAuthenticationRepository>()
                    .Setup(x => x.AuthenticateUser(user.UserId, "Random"))
                    .Returns(Task.FromResult(false));

                //Create instance of class and call method
                var testClass = mock.Create<AuthenticationService>();
                await testClass.HandleCommandAsync(command);

                var expected = new UnauthorizedObjectResult("Password is incorrect");

                UnauthorizedObjectResult actual = (UnauthorizedObjectResult)await testClass.HandleCommandAsync(command);

                Assert.Equal(expected.Value, actual.Value);
            }
        }

        [Fact]
        public async Task HandleCommand_LoginUser_BadRequest()
        {
            LoginUserCommand command = new()
            {
                EmailAddress = "email@email.com",
                Password = "password"
            };

            using (var mock = AutoMock.GetLoose())
            {                
                //Create instance of class and call method
                var testClass = mock.Create<AuthenticationService>();
                await testClass.HandleCommandAsync(command);

                var expected = new BadRequestObjectResult("No account for this email address");

                BadRequestObjectResult actual = (BadRequestObjectResult)await testClass.HandleCommandAsync(command);

                Assert.Equal(expected.Value, actual.Value);
            }
        }

        [Fact]
        public async Task HandleCommand_RefreshToken()
        {
            IConfiguration configuration = SampleConfiguration();
            var tokenHandler = new JwtSecurityTokenHandler();
            User user = GetSampleUser();
            var token = GenerateToken(user.UserId, configuration, true);
            var tokenWritten = tokenHandler.WriteToken(token);
            
            RefreshJwtTokenCommand command = new RefreshJwtTokenCommand
            {
                UserId = user.UserId,
                Token = tokenWritten,
                RefreshToken = GenerateRefreshToken()
            };
            DateTime refreshExpires = DateTime.UtcNow.AddDays(1);   
            user.RefreshToken = command.RefreshToken;
            user.RefreshExpires = refreshExpires;

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"]));
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = securityKey
            };

            var authMock = new Mock<IAuthenticationRepository>();
            var expected = new AuthResultVM
            {
                Token = tokenWritten,
                RefreshToken = user.RefreshToken,
                ExpiresAt = DateTime.Today.AddDays(1)
            };
            authMock.Setup(x => x.GetRefreshToken(user.UserId))
                    .Returns(Task.FromResult(new OkObjectResult
                            (new Tuple<string, DateTime>
                            (user.RefreshToken, user.RefreshExpires)) as IActionResult));

            var testClass = new AuthenticationService(null, configuration, tokenValidationParameters, authMock.Object, null, null);

            OkObjectResult result = (OkObjectResult)await testClass.HandleCommandAsync(command);
            AuthResultVM? actual = result.Value as AuthResultVM;

            Assert.Equal(expected.ExpiresAt, actual.ExpiresAt);
        }

        [Fact]
        public async Task HandleCommand_RefreshToken_BadRequest_InvalidPrincipal()
        {            
            RefreshJwtTokenCommand command = GetSampleToken();
            command.Token = null;

            using (var mock = AutoMock.GetLoose())
            {
                var testClass = mock.Create<AuthenticationService>();
                await testClass.HandleCommandAsync(command);

                var expected = new BadRequestObjectResult("Invalid request");

                BadRequestObjectResult actual = (BadRequestObjectResult)await testClass.HandleCommandAsync(command);

                Assert.Equal(expected.Value, actual.Value);
            }
        }

        [Fact]
        public async Task HandleCommand_RefreshToken_BadRequest_InvalidToken()
        {
            IConfiguration configuration = SampleConfiguration();
            var tokenHandler = new JwtSecurityTokenHandler();
            User user = GetSampleUser();
            RefreshJwtTokenCommand command = new RefreshJwtTokenCommand
            {
                UserId = user.UserId,
                Token = tokenHandler.WriteToken(GenerateToken(user.UserId, configuration, true)),
                RefreshToken = GenerateRefreshToken()
            };            
            user.RefreshToken = command.RefreshToken;
            user.RefreshExpires = DateTime.Now.AddDays(-1);

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"]));
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = securityKey
            };

            var authMock = new Mock<IAuthenticationRepository>();
            authMock.Setup(x => x.GetRefreshToken(user.UserId))
                    .Returns(Task.FromResult(
                            new OkObjectResult(new Tuple<string, DateTime>(user.RefreshToken, user.RefreshExpires)) 
                            as IActionResult));
            
            var testClass = new AuthenticationService(null, configuration, tokenValidationParameters, authMock.Object, null, null);

            var expected = new BadRequestObjectResult("Invalid refresh token");

            var actual = (BadRequestObjectResult)await testClass.HandleCommandAsync(command);

            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public async Task HandleCommand_RefreshToken_SecurityException()
        {
            IConfiguration configuration = SampleConfiguration();
            RefreshJwtTokenCommand command = GetSampleToken();
            JwtSecurityToken token = GenerateToken(command.UserId, configuration, false);
            var tokenHandler = new JwtSecurityTokenHandler();
            command.Token = tokenHandler.WriteToken(token);
            command.RefreshToken = GenerateRefreshToken().ToString();

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"]));
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = securityKey
            };

            var testClass = new AuthenticationService(null, configuration, tokenValidationParameters, null, null, null);

            await Assert.ThrowsAsync<SecurityTokenException>(() => testClass.HandleCommandAsync(command));
        }

        private LoginUserCommand GetLoginCommand()
        {
            LoginUserCommand command = new()
            {
                EmailAddress = "email@email.com",
                Password = "password"
            };

            return command;
        }

        private User GetSampleUser()
        {
            User user = new()
            {
                EmailAddress = "gg@gg.com",
                Password = "password",
                Forename = "Joe",
                Surname = "Else"
            };

            return user;
        }

        private RefreshJwtTokenCommand GetSampleToken()
        {
            RefreshJwtTokenCommand command = new()
            {
                Token = "blah",
                RefreshToken = "bluh",
                UserId = 23
            };

            return command;
        }

        private IConfiguration SampleConfiguration()
        {
            IdentityModelEventSource.ShowPII = true;

            Random rnd = new Random();
            byte[] b = new byte[16];
            rnd.NextBytes(b);

            string secret = Convert.ToBase64String(b);

            var myConfiguration = new Dictionary<string, string>
                {
                    {"JWT:Issuer", "Me"},
                    {"JWT:Audience", "enduser"},
                    {"JWT:Secret", secret}
                };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            return configuration;
        }

        private RefreshJwtTokenCommand GetTokenCommand()
        {
            RefreshJwtTokenCommand refreshToken = new()
            {
                Token = "Blah",
                RefreshToken = "Bluh",
                UserId = 23
            };

            return refreshToken;
        }

        private JwtSecurityToken GenerateToken(int userId, IConfiguration configuration, bool validAlgorithm)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"]));
            var expiresAt = DateTime.UtcNow.AddMinutes(20);
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var algorithm = SecurityAlgorithms.HmacSha256;

            if (!validAlgorithm)
            {
                algorithm = SecurityAlgorithms.HmacSha384;
            }

            //Create Security Token
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: expiresAt,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, algorithm));
            
            return token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
