using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BasicSubscriber
{
    internal class Worker : BackgroundService
    {
        private ISubscriptionClient subscriptionClient;

        private readonly ILogger<Worker> logger;
        private readonly ServiceBusSettings serviceBusSettings;

        public Worker(ILogger<Worker> logger, IOptions<ServiceBusSettings> options)
        {
            this.logger = logger;
            serviceBusSettings = options.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            subscriptionClient = new SubscriptionClient(serviceBusSettings.ConnectionString, serviceBusSettings.Topic, serviceBusSettings.Subscription);

            RegisterMessageHandler();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await subscriptionClient.CloseAsync();

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            logger.LogInformation($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            logger.LogError($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            logger.LogError($"Exception context for troubleshooting:\n- Endpoint: {context.Endpoint}\n- Entity Path: {context.EntityPath}\n- Executing Action: {context.Action}");

            return Task.CompletedTask;
        }
    }
}
