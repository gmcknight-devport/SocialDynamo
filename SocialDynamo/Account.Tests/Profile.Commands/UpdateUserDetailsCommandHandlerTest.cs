using Common.API.Infrastructure.Repositories;
using Common.API.Profile.Commands;
using Common.Domain.Repositories;
using Common.Models.Users;
using Autofac.Extras.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Common.Tests.Profile.Commands
{
    public class UpdateUserDetailsCommandHandlerTest
    {
        [Fact]
        public async Task Handle_UpdatesUserForenameToRepoAsync()
        {
            User user = GetSampleUser();
            string newForename = "new forename";
            UpdateUserDetailsCommand command = new()
            {
                UserId = user.UserId,
                Forename = user.Forename
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
                var testClass = mock.Create<UpdateUserDetailsCommandHandler>();
                _ = await testClass.Handle(command, token);
                user.Forename = newForename;

                //Verify method on mock was called once
                mock.Mock<IUserRepository>()
                    .Verify(x => x.UpdateUserAsync(It.Is<User>(u => u.UserId == user.UserId &&
                                                               u.Forename == user.Forename &&
                                                               u.Surname == user.Surname)),
                                                               Times.Exactly(1));
            }
        }

        [Fact]
        public async Task Handle_UpdatesUserSurnameToRepoAsync()
        {
            User user = GetSampleUser();
            string newSurname = "Well isn't this different";
            UpdateUserDetailsCommand command = new()
            {
                UserId = user.UserId,
                Surname = user.Surname
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
                var testClass = mock.Create<UpdateUserDetailsCommandHandler>();
                _ = await testClass.Handle(command, token);
                user.Surname = newSurname;

                //Verify method on mock was called once
                mock.Mock<IUserRepository>()
                    .Verify(x => x.UpdateUserAsync(It.Is<User>(u => u.UserId == user.UserId &&
                                                               u.Forename == user.Forename &&
                                                               u.Surname == user.Surname)),
                                                               Times.Exactly(1));
            }
        }

        [Fact]
        public async Task Handle_UpdatesUserNamesToRepoAsync()
        {
            User user = GetSampleUser();
            string newForename = "Fore namington";
            string newSurname = "Sursursur";
            UpdateUserDetailsCommand command = new()
            {
                UserId = user.UserId,
                Forename = user.Surname,
                Surname = user.Surname
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
                var testClass = mock.Create<UpdateUserDetailsCommandHandler>();
                _ = await testClass.Handle(command, token);
                user.Forename = newForename;
                user.Surname = newSurname;

                //Verify method on mock was called once
                mock.Mock<IUserRepository>()
                    .Verify(x => x.UpdateUserAsync(It.Is<User>(u => u.UserId == user.UserId &&
                                                               u.Forename == newForename &&
                                                               u.Surname == newSurname)),
                                                               Times.Exactly(1));
            }
        }
        private User GetSampleUser()
        {
            User sampleUser = new()
            {
                UserId = "AnId23",
                Password = "password",
                Forename = "Phil",
                Surname = "Philington"
            };

            return sampleUser;
        }
    }
}
