using Azure.Messaging.ServiceBus;
using Common;
using MediatR;
using Newtonsoft.Json;
using Posts.API.IntegrationEvents;
using Posts.Domain.Repositories;
using System.Net.Mime;
using System.Text;

namespace Posts.API.Commands
{
    //Command handler to delete a specified post. 
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<DeletePostCommandHandler> _logger;
        private readonly IConfiguration _configuration;

        public DeletePostCommandHandler(IPostRepository postRepository, 
                                        ILogger<DeletePostCommandHandler> logger, 
                                        IConfiguration configuration)
        {
            _postRepository = postRepository;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Handle method of mediatr interface - deletes the specified post.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(DeletePostCommand command, CancellationToken cancellationToken)
        {
            var mediaItemIds = _postRepository.GetPostAsync(command.PostId).Result.MediaItemIds;
            await _postRepository.DeletePostAsync(command.PostId);

            if (mediaItemIds.Any())
            {
                List<string> mediaList = new();
                foreach(var v in mediaItemIds)
                {
                    mediaList.Add(Convert.ToString(v));
                }

                PostDeletedIntegrationEvent integrationEvent = new()
                {
                    MediaItemIds = mediaList
                };
                PublishIntegrationEvent(integrationEvent);
            }

            _logger.LogInformation("----- Specified post deleted. Post: {@PostId}", command.PostId);
            
            return true;
        }

        private async void PublishIntegrationEvent(IIntegrationEvent integrationEvent)
        {
            var jsonMessage = JsonConvert.SerializeObject(integrationEvent);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            var client = new ServiceBusClient(_configuration["ConnectionStrings:ServiceBus"]);
            var sender = client.CreateSender(integrationEvent.GetType().Name);

            var message = new ServiceBusMessage()
            {
                Body = new BinaryData(body),
                MessageId = Guid.NewGuid().ToString(),
                ContentType = MediaTypeNames.Application.Json,
                Subject = integrationEvent.GetType().Name
            };

            await sender.SendMessageAsync(message);
            _logger.LogInformation("----- New PostDeletedIntegrationEvent created and send. " +
                "MessageId: {@MessageId}, Body: {@Body}", message.MessageId, message.Body);
        }
    }
}
