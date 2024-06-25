using Common.API.Infrastructure.Repositories;
using Common.API.Profile.Commands;
using Common.API.Services;
using Common.Domain.Repositories;
using Common.Models.Users;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Common.Tests.Profile.Commands
{
    public class ChangePasswordCommandHandlerTest
    {
        [Fact]
        public async Task Handle_UpdatesPasswordToRepoAsync()
        {
            User user = GetSampleUser();
            string newPassword = "Elbow1";

            ChangePasswordCommand command = new()
            {
                UserId = user.UserId,
                OldPassword = "Password",
                NewPassword = newPassword
            };
            CancellationToken token = new();

            using (var mock = AutoMock.GetLoose())
            {
                //Mock repository and method within used in test
                mock.Mock<IUserRepository>()
                    .Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                    .Verifiable();
                                
                mock.Mock<IUserRepository>()
                    .Setup(x => x.GetUserAsync(user.UserId))
                    .Returns(Task.FromResult(user));

                mock.Mock<IAuthenticationService>()
                    .Setup(x => x.HashPassword("Password"))
                    .Returns(user.Password);
                                
                //Create instance of class and call method
                var testClass = mock.Create<ChangePasswordCommandHandler>();
                _ = await testClass.Handle(command, token);
                user.Password = HashPassword(newPassword);

                //Verify method on mock was called once
                mock.Mock<IUserRepository>()
                    .Verify(x => x.UpdateUserAsync(It.Is<User>(u => u.UserId == user.UserId &&
                                                               u.Password == HashPassword(newPassword) &&
                                                               u.Forename == user.Forename &&
                                                               u.Surname == user.Surname)), 
                                                               Times.Exactly(1));
            }
        }

        private User GetSampleUser()
        {
            User sampleUser = new()
            {
                UserId = "AnId22",
                Password = HashPassword("password"),
                Forename = "Phil",
                Surname = "Philington"
            };

            return sampleUser;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var asBytes = Encoding.Default.GetBytes(password);
            var hashed = sha.ComputeHash(asBytes);

            return Convert.ToBase64String(hashed);
        }
    }
}
