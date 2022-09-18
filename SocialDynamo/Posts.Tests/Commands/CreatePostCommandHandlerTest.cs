using Autofac.Extras.Moq;
using Moq;
using Posts.API.Commands;
using Posts.Domain.Models;
using Posts.Domain.ValueObjects;
using Posts.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Posts.Tests.Commands
{
    public class CreatePostCommandHandlerTest
    {
        [Fact]
        public async Task Handle_CreatesNewPost()
        {
            CreatePostCommand command = new()
            {
                AuthorId = "20",
                Hashtag = "#tag",
                Caption = "Post caption",
                MediaItemIds = new()
                {
                    MediaItemId.Create("20"),
                    MediaItemId.Create("20"),
                    MediaItemId.Create("20")
                }
            };
            CancellationToken token = new CancellationToken();

            using(var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                    .Setup(x => x.CreatePostAsync(It.IsAny<Post>()))
                    .Verifiable();

                var testClass = mock.Create<CreatePostCommandHandler>();
                await testClass.Handle(command, token);

                mock.Mock<IPostRepository>()
                    .Verify(x => x.CreatePostAsync(It.Is<Post>(p => p.AuthorId == command.AuthorId &&
                                                               p.Hashtag == command.Hashtag &&
                                                               p.Caption == command.Caption &&
                                                               p.MediaItemIds == command.MediaItemIds)),
                                                               Times.Exactly(1));
            }
        }
    }
}
