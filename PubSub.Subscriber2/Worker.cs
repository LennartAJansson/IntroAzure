using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PubSub.Subscriber2
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IBusControl bus;

        public Worker(ILogger<Worker> logger, IBusControl bus)
            : base()
        {
            this.logger = logger;
            this.bus = bus;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            await bus.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await bus.StopAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
