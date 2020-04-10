using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BasicPublisher
{
    internal class Worker : BackgroundService
    {
        private ITopicClient topicClient;
        private Timer timer;
        private int counter = 0;

        private readonly ILogger<Worker> logger;
        private readonly TimerSettings timerSettings;
        private readonly ServiceBusSettings serviceBusSettings;

        public Worker(ILogger<Worker> logger, IOptions<TimerSettings> timerOptions, IOptions<ServiceBusSettings> options)
        {
            this.logger = logger;
            timerSettings = timerOptions.Value;
            serviceBusSettings = options.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            topicClient = new TopicClient(serviceBusSettings.ConnectionString, serviceBusSettings.Topic);

            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(timerSettings.TimerSeconds));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            await topicClient.CloseAsync();

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            timer?.Dispose();

            base.Dispose();
        }

        private async void DoWork(object state)
        {
            try
            {
                string messageBody = $"Message {counter++}";

                var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                logger.LogInformation($"Sending message: {messageBody}");

                await topicClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                logger.LogError($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
