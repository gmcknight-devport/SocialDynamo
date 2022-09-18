using Autofac.Extras.Moq;
using Moq;
using Posts.API.Commands;
using Posts.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Posts.Tests.Commands
{
    public class LikeCommentCommandHandlerTest
    {
        [Fact]
        public async Task Handle_LikeComment()
        {
            LikeCommentCommand command = new()
            {
                LikeUserId = "422",
                CommentId = Guid.NewGuid()
            };
            CancellationToken token = new CancellationToken();

            using(var mock = AutoMock.GetLoose())
            {
                mock.Mock<ICommentRepository>()
                   .Setup(c => c.LikeCommentAsync(command.CommentId, command.LikeUserId))
                   .Verifiable();

                var testClass = mock.Create<LikeCommentCommandHandler>();
                await testClass.Handle(command, token);

                mock.Mock<ICommentRepository>()
                                .Verify(x => x.LikeCommentAsync(command.CommentId, command.LikeUserId), Times.Exactly(1));
            }
        }
    }
}
