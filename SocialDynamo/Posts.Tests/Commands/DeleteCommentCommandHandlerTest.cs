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
    public class DeleteCommentCommandHandlerTest
    {
        [Fact]
        public async Task Handle_DeletesAComment()
        {
            DeleteCommentCommand command = new()
            {
                CommentId = Guid.NewGuid()
            };
            CancellationToken token = new CancellationToken();  

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<ICommentRepository>()
                    .Setup(c => c.DeleteCommentAsync(command.CommentId))
                    .Verifiable();

                var testClass = mock.Create<DeleteCommentCommandHandler>();
                await testClass.Handle(command, token);

                mock.Mock<ICommentRepository>()
                    .Verify(x => x.DeleteCommentAsync(command.CommentId), Times.Exactly(1));
            }
        }
    }
}
