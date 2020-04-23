using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BasicEventHubSender
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly EventHubSettings eventHubSettings;

        public Worker(ILogger<Worker> logger, IOptions<EventHubSettings> options)
        {
            this.logger = logger;
            this.eventHubSettings = options.Value;
        }

        public override Task StartAsync(CancellationToken cancellationToken) => base.StartAsync(cancellationToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                // Create a producer client that you can use to send events to an event hub
                await using (var producerClient = new EventHubProducerClient(eventHubSettings.EventHubConnectionString, eventHubSettings.EventHubName))
                {
                    // Create a batch of events 
                    using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                    // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("First event")));
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Second event")));
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Third event")));

                    // Use the producer client to send the batch of events to the event hub
                    await producerClient.SendAsync(eventBatch);
                    Console.WriteLine("A batch of 3 events has been published.");
                }
                await Task.Delay(10000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken) => base.StopAsync(cancellationToken);
    }
}
