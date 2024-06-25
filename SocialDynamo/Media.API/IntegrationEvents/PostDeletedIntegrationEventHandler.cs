using Azure.Messaging.ServiceBus;
using Common.OptionsConfig;
using Media.API.Commands;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Media.API.IntegrationEvents
{
    public class PostDeletedIntegrationEventHandler : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PostDeletedIntegrationEventHandler> _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly string _queueName = "PostDeletedIntegrationEvent";

        public PostDeletedIntegrationEventHandler(IConfiguration baseConfiguration,
                                                  IOptions<ConnectionOptions> optionsConfiguration,
                                                  IServiceScopeFactory serviceScopeFactory,
                                                  ILogger<PostDeletedIntegrationEventHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            //Validation workaround to allow connectionstring to be found in dev or prod.
            if (baseConfiguration["ServiceBus"] != null)
                _client = new ServiceBusClient(baseConfiguration["ServiceBus"]);
            else
                _client = new ServiceBusClient(optionsConfiguration.Value.ServiceBus);

            _processor = _client.CreateProcessor(_queueName);
            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processor.StartProcessingAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
        }

        private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            var theEvent = JsonConvert.DeserializeObject<PostDeletedIntegrationEvent>(body);
            await args.CompleteMessageAsync(args.Message);

            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            List<DeleteBlobCommand> commands = new();
            
            try
            {
                foreach (var i in theEvent.MediaItemIds)
                {
                    DeleteBlobCommand command = new()
                    {
                        UserId = theEvent.UserId,
                        MediaItemId = i
                    };
                    commands.Add(command);
                };

                foreach (var i in commands)
                {
                    bool executed = await mediator.Send(i);
                }
                _logger.LogInformation("----- Post deleted integrationevent received. " +
                    "MediaItemIds: {@MediaItemIds}", theEvent.MediaItemIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
