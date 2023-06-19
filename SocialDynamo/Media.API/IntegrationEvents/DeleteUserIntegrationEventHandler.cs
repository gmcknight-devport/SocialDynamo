using Azure.Messaging.ServiceBus;
using Common.OptionsConfig;
using Media.API.Commands;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Media.API.IntegrationEvents
{
    public class DeleteUserIntegrationEventHandler : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DeleteUserIntegrationEventHandler> _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly string _queueName = "DeleteUserIntegrationEvent";

        public DeleteUserIntegrationEventHandler(IConfiguration baseConfiguration,
                                                 IOptions<ConnectionOptions> optionsConfiguration,
                                                 IServiceScopeFactory serviceScopeFactory, 
                                                 ILogger<DeleteUserIntegrationEventHandler> logger)
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
            var theEvent = JsonConvert.DeserializeObject<DeleteUserIntegrationEvent>(body);
            await args.CompleteMessageAsync(args.Message);

            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
             
            try
            {
                var command = new DeleteUserContainerCommand
                {
                    UserId = theEvent.UserId
                };

                bool executed = await mediator.Send(command);

                _logger.LogInformation("----- User deleted integration event received. " +
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
