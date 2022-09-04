using Account.API.Infrastructure.Repositories;
using Account.API.Profile.Commands;
using Account.Models.Users;
using Autofac.Extras.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Account.Tests.Profile.Commands
{
    public class UpdateProfileDescriptionCommandHandlerTest
    {
        [Fact]
        public async Task Handle_UpdatesProfileDescriptionToRepoAsync()
        {
            User user = GetSampleUser();
            string newDescription = "This is the new description";
            UpdateProfileDescriptionCommand command = new()
            {
                UserId = user.UserId,
                Description = "Init description"
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
                var testClass = mock.Create<UpdateProfileDescriptionCommandHandler>();
                _ = await testClass.Handle(command, token);
                user.ProfileDescription = newDescription;

                //Verify method on mock was called once
                mock.Mock<IUserRepository>()
                    .Verify(x => x.UpdateUserAsync(It.Is<User>(u => u.UserId == user.UserId &&
                                                               u.Forename == user.Forename &&
                                                               u.Surname == user.Surname &&
                                                               u.ProfileDescription == newDescription)),
                                                               Times.Exactly(1));
            }
        }

        private User GetSampleUser()
        {
            User sampleUser = new()
            {
                UserId = 233,
                Password = "password",
                Forename = "Phil",
                Surname = "Philington"
            };

            return sampleUser;
        }
    }
}
