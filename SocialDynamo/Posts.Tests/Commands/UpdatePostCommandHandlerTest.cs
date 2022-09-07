using Autofac.Extras.Moq;
using Moq;
using Posts.API.Commands;
using Posts.Domain.Models;
using Posts.Domain.ValueObjects;
using Posts.Infrastructure.Repositories;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Posts.Tests.Commands
{
    public class UpdatePostCommandHandlerTest
    {
        private readonly ITestOutputHelper _output;

        public UpdatePostCommandHandlerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Handle_UpdatePost_ThrowsNullException()
        {
            Post post = new()
            {
                PostId = Guid.NewGuid(),
                AuthorId = 11,
                PostedAt = DateTime.UtcNow,
                Likes = new List<PostLike>(),
                Comments = new List<Comment>(),
                MediaItemIds = new List<MediaItemId>()
            };
            UpdatePostCommand command = new()
            {
                PostId = Guid.NewGuid()
            };
            CancellationToken token = new();

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                       .Setup(p => p.GetPostAsync(command.PostId))
                       .Returns(Task.FromResult(post));

                var testClass = mock.Create<UpdatePostCommandHandler>();

                await Assert.ThrowsAsync<ArgumentNullException>(async () => await testClass.Handle(command, token));
            }
        }

        [Fact]
        public async Task Handle_UpdatePost_CaptionOnly()
        {
            Post originalPost = new()
            {
                PostId = Guid.NewGuid(),
                AuthorId = 11,
                PostedAt = DateTime.UtcNow,
                Likes = new List<PostLike>(),
                Comments = new List<Comment>(),
                MediaItemIds = new List<MediaItemId>(),
                Caption = "Old caption :("
            };            

            UpdatePostCommand command = new()
            {
                PostId = originalPost.PostId,
                Caption = "New caption yeah"
            };

            Post newPost = originalPost;
            newPost.Caption = command.Caption;

            CancellationToken token = new();

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                       .Setup(p => p.GetPostAsync(command.PostId))
                       .Returns(Task.FromResult(originalPost));

                mock.Mock<IPostRepository>()
                    .Setup(p => p.UpdatePostAsync(It.IsAny<Post>()))
                    .Verifiable();

                var testClass = mock.Create<UpdatePostCommandHandler>();
                await testClass.Handle(command, token);

                mock.Mock<IPostRepository>()
                    .Verify(p => p.UpdatePostAsync(It.Is<Post>(p => p.PostId == command.PostId &&
                                                              p.AuthorId == originalPost.AuthorId &&
                                                              p.Caption == newPost.Caption &&
                                                              p.MediaItemIds == originalPost.MediaItemIds)),
                                                              Times.Exactly(1));
            }
        }

        [Fact]
        public async Task Handle_UpdatePost_MediaItemsOnly()
        {
            Post originalPost = new()
            {
                PostId = Guid.NewGuid(),
                AuthorId = 11,
                PostedAt = DateTime.UtcNow,
                Likes = new List<PostLike>(),
                Comments = new List<Comment>(),
                MediaItemIds = new List<MediaItemId>()
                {                    
                    MediaItemId.Create(11),
                    MediaItemId.Create(11),
                },
                Caption = "Old caption :("
            };
            
            UpdatePostCommand command = new()
            {
                PostId = originalPost.PostId,
                MediaItemIds = new List<MediaItemId>()
                {
                    MediaItemId.Create(998237289),
                    MediaItemId.Create(123456789),
                }
            };
            Post newPost = originalPost;
            newPost.MediaItemIds = command.MediaItemIds;

            CancellationToken token = new();

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                       .Setup(p => p.GetPostAsync(command.PostId))
                       .Returns(Task.FromResult(originalPost));

                mock.Mock<IPostRepository>()
                    .Setup(p => p.UpdatePostAsync(It.IsAny<Post>()))
                    .Verifiable();

                var testClass = mock.Create<UpdatePostCommandHandler>();
                await testClass.Handle(command, token);
                               
                mock.Mock<IPostRepository>()
                    .Verify(p => p.UpdatePostAsync(It.Is<Post>(p => p.PostId == command.PostId &&
                                                              p.AuthorId == originalPost.AuthorId &&
                                                              p.Caption == originalPost.Caption &&
                                                              newPost.MediaItemIds.All(p.MediaItemIds.Contains))),
                                                              Times.Exactly(1));
            }
        }

        [Fact]
        public async Task Handle_UpdatePost_AllFields()
        {
            Post originalPost = new()
            {
                PostId = Guid.NewGuid(),
                AuthorId = 11,
                PostedAt = DateTime.UtcNow,
                Likes = new List<PostLike>(),
                Comments = new List<Comment>(),
                MediaItemIds = new List<MediaItemId>()
                {
                    MediaItemId.Create(11),
                    MediaItemId.Create(11),
                },
                Caption = "Old caption :("
            };

            UpdatePostCommand command = new()
            {
                PostId = originalPost.PostId,
                Caption = "Nice new caption",
                MediaItemIds = new List<MediaItemId>()
                {
                    MediaItemId.Create(998237289),
                    MediaItemId.Create(123456789),
                }
            };
            Post newPost = originalPost;
            newPost.MediaItemIds = command.MediaItemIds;
            newPost.Caption = command.Caption;

            CancellationToken token = new();

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                       .Setup(p => p.GetPostAsync(command.PostId))
                       .Returns(Task.FromResult(originalPost));

                mock.Mock<IPostRepository>()
                    .Setup(p => p.UpdatePostAsync(It.IsAny<Post>()))
                    .Verifiable();

                var testClass = mock.Create<UpdatePostCommandHandler>();
                await testClass.Handle(command, token);

                mock.Mock<IPostRepository>()
                    .Verify(p => p.UpdatePostAsync(It.Is<Post>(p => p.PostId == command.PostId &&
                                                              p.AuthorId == originalPost.AuthorId &&
                                                              p.Caption == newPost.Caption &&
                                                              newPost.MediaItemIds.All(p.MediaItemIds.Contains))),
                                                              Times.Exactly(1));
            }
        }
    }
}
