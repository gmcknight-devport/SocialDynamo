using Autofac.Extras.Moq;
using Moq;
using Posts.API.Commands;
using Posts.Infrastructure.Repositories;
using Xunit;

namespace Posts.Tests.Commands
{
    public class DeletePostCommandHandlerTest
    {
        [Fact]
        public async Task Handle_DeletePost()
        {
            DeletePostCommand command = new()
            {
                PostId = Guid.NewGuid()
            };
            CancellationToken token = new CancellationToken();

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                    .Setup(c => c.DeletePostAsync(command.PostId))
                    .Verifiable();

                var testClass = mock.Create<DeletePostCommandHandler>();
                await testClass.Handle(command, token);

                mock.Mock<IPostRepository>()
                                .Verify(x => x.DeletePostAsync(command.PostId), Times.Exactly(1));
            }
        }
    }
}
