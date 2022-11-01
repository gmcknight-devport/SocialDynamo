using Autofac.Extras.Moq;
using Moq;
using Posts.API.Commands;
using Posts.Domain.Models;
using Posts.Domain.Repositories;
using Posts.Domain.ValueObjects;
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
            Post post = new()
            {
                PostId = command.PostId,
                MediaItemIds = new List<MediaItemId>()
            };
            CancellationToken token = new CancellationToken();

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                    .Setup(c => c.DeletePostAsync(command.PostId))
                    .Verifiable();

                mock.Mock<IPostRepository>()
                    .Setup(c => c.GetPostAsync(command.PostId))
                    .Returns(Task.FromResult(post));

                var testClass = mock.Create<DeletePostCommandHandler>();
                await testClass.Handle(command, token);

                mock.Mock<IPostRepository>()
                                .Verify(x => x.DeletePostAsync(command.PostId), Times.Exactly(1));
            }
        }
    }
}
