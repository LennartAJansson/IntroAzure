using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BasicEventHubListener
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly EventHubSettings eventHubSettings;

        //private BlobContainerClient storageClient;
        //private EventProcessorClient processor;
        private const string blobStorageConnectionString = "<AZURE STORAGE CONNECTION STRING>";
        private const string blobContainerName = "<BLOB CONTAINER NAME>";

        public Worker(ILogger<Worker> logger, IOptions<EventHubSettings> options)
        {
            _logger = logger;
            this.eventHubSettings = options.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // Read from the default consumer group: $Default
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Create a blob container client that the event processor will use 
            //storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

            // Create an event processor client to process events in the event hub
            //processor = new EventProcessorClient(storageClient, consumerGroup, eventHubSettings.ConnectionString, eventHubSettings.Event);

            // Register handlers for processing events and handling errors
            //processor.ProcessEventAsync += ProcessEventHandler;
            //processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            //await processor.StartProcessingAsync();

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                // Wait for 10 seconds for the events to be processed
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop the processing
            //await processor.StopProcessingAsync();
            await base.StopAsync(cancellationToken);
        }

        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // Write the body of the event to the console window
            Console.WriteLine("\tRecevied event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
