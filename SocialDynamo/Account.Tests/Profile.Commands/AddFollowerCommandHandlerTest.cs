using Account.API.Infrastructure.Repositories;
using Account.API.Profile.Commands;
using Account.Domain.Repositories;
using Account.Models.Users;
using Autofac.Extras.Moq;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Account.Tests.Profile.Commands
{
    public class AddFollowerCommandHandlerTest
    {
        private readonly ITestOutputHelper _output;

        public AddFollowerCommandHandlerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Handle_AddsFollowerToDBAsync()
        {
            Follower follower = GetSampleFollower();
            AddFollowerCommand command = new()
            {
                UserId = follower.UserId,
                FollowerId = follower.FollowerId
            };
            CancellationToken token = new();

            using (var mock = AutoMock.GetLoose())
            {                
                //Mock repository and method within used in test
                mock.Mock<IFollowerRepository>()
                    .Setup(x => x.AddFollower(follower.UserId, It.IsAny<Follower>()))
                    .Verifiable();

                //Create instance of class and call method
                var testClass = mock.Create<AddFollowerCommandHandler>();
                await testClass.Handle(command, token);

                //Verify method on mock was called once
                mock.Mock<IFollowerRepository>()
                    .Verify(x => x.AddFollower(follower.UserId, It.Is<Follower>(f => f.UserId == follower.UserId && 
                                                              f.FollowerId == follower.FollowerId)), 
                                                              Times.Exactly(1));
            }
        }

        private Follower GetSampleFollower()
        {
            string followerId = "1892";
            string userId = "12";
            Follower sampleFollower = new()
            {
                UserId = userId,
                FollowerId = followerId
            };

            return sampleFollower;
        }
    }
}
