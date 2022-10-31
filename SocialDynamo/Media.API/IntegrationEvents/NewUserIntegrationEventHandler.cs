using Azure.Messaging.ServiceBus;
using Media.API.Commands;
using MediatR;
using Newtonsoft.Json;

namespace Media.API.IntegrationEvents
{
    public class NewUserIntegrationEventHandler : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<NewUserIntegrationEventHandler> _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly string _queueName = "NewUserIntegrationEvent";

        public NewUserIntegrationEventHandler(IConfiguration configuration,
                                              IServiceScopeFactory serviceScopeFactory, 
                                              ILogger<NewUserIntegrationEventHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            _client = new ServiceBusClient(configuration["ConnectionStrings:ServiceBus"]);
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
            var theEvent = JsonConvert.DeserializeObject<NewUserIntegrationEvent>(body);
            await args.CompleteMessageAsync(args.Message);

            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var command = new AddUserBlobContainerCommand
            {
                UserId = theEvent.UserId
            };

            try
            {
                bool executed = await mediator.Send(command);

                _logger.LogInformation("----- New user integration event received. " +
                    "User: {@UserId}", theEvent.UserId);
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
