using Account.API.Infrastructure.Repositories;
using Account.API.Profile.Commands;
using Account.Models.Users;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Account.Tests.Profile.Commands
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
                Password = newPassword
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
                                
                //Create instance of class and call method
                var testClass = mock.Create<ChangePasswordCommandHandler>();
                _ = await testClass.Handle(command, token);
                user.Password = newPassword;

                //Verify method on mock was called once
                mock.Mock<IUserRepository>()
                    .Verify(x => x.UpdateUserAsync(It.Is<User>(u => u.UserId == user.UserId &&
                                                               u.Password == newPassword &&
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
                Password = "password",
                Forename = "Phil",
                Surname = "Philington"
            };

            return sampleUser;
        }
    }
}
