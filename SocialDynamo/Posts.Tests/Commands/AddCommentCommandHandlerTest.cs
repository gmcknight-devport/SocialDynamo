using Autofac.Extras.Moq;
using Moq;
using Posts.API.Commands;
using Posts.Domain.Models;
using Posts.Domain.ValueObjects;
using Posts.Infrastructure.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Posts.Tests.Commands
{
    public  class AddCommentCommandHandlerTest
    {
        [Fact]
        public async Task Handle_AddsCommentToPost()
        {            
            Post post = GetSamplePost();
            AddCommentCommand command = new()
            {
                PostId = post.PostId,
                AuthorId = post.AuthorId,
                Comment = "New comment here"
            };
            CancellationToken token = new();

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                    .Setup(x => x.GetPostAsync(post.PostId))
                    .Returns(Task.FromResult(post));

                mock.Mock<ICommentRepository>()
                    .Setup(c => c.AddCommentAsync(post.PostId, It.IsAny<Comment>()))
                    .Verifiable();

                var testClass = mock.Create<AddCommentCommandHandler>();
                await testClass.Handle(command, token);

                mock.Mock<ICommentRepository>()
                    .Verify(x => x.AddCommentAsync(post.PostId, It.Is<Comment>(c => c.AuthorId == command.AuthorId &&
                                                              c.CommentText == command.Comment)),
                                                              Times.Exactly(1));

            }
        }

        private Post GetSamplePost()
        {
            Post post = new()
            {
                AuthorId = "20",
                Caption = "Captione",
                PostedAt = DateTime.UtcNow,
                MediaItemIds = new List<MediaItemId>(),
                Likes = new List<PostLike>(),
                Comments = new List<Comment>()
            };

            return post;
        }
    }
}
