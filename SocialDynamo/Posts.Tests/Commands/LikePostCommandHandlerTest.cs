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
using Xunit.Abstractions;

namespace Posts.Tests.Commands
{
    public class LikePostCommandHandlerTest
    {
        [Fact]
        public async Task Handle_LikeComment()
        {
            LikePostCommand command = new()
            {
                LikeUserId = 422,
                PostId = Guid.NewGuid()
            };
            CancellationToken token = new CancellationToken();

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                   .Setup(c => c.LikePostAsync(command.PostId, command.LikeUserId))
                   .Verifiable();

                var testClass = mock.Create<LikePostCommandHandler>();
                await testClass.Handle(command, token);

                mock.Mock<IPostRepository>()
                                .Verify(x => x.LikePostAsync(command.PostId, command.LikeUserId), Times.Exactly(1));
            }
        }
    }
}
