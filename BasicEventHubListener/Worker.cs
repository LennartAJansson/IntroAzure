using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BasicEventHubListener
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly EventHubSettings eventHubSettings;

        private BlobContainerClient storageClient;
        private EventProcessorClient processor;

        public Worker(ILogger<Worker> logger, IOptions<EventHubSettings> options)
        {
            this.logger = logger;
            this.eventHubSettings = options.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            // Read from the default consumer group: $Default
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Create a blob container client that the event processor will use 
            storageClient = new BlobContainerClient(eventHubSettings.BlobConnectionString, eventHubSettings.BlobContainerName.ToLower());

            // Create an event processor client to process events in the event hub
            processor = new EventProcessorClient(storageClient, consumerGroup, eventHubSettings.EventHubConnectionString, eventHubSettings.EventHubName);

            // Start the processing
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;
            await processor.StartProcessingAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop the processing
            await processor.StopProcessingAsync(cancellationToken);
            processor.ProcessEventAsync -= ProcessEventHandler;
            processor.ProcessErrorAsync -= ProcessErrorHandler;

            await base.StopAsync(cancellationToken);
        }

        private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // Write the body of the event to the console window
            logger.LogInformation("Recevied event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            logger.LogError(eventArgs.Exception, $"Partition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");

            return Task.CompletedTask;
        }
    }
}
