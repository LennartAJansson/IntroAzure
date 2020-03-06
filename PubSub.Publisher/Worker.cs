using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using PubSub.Common.Abstract;
using PubSub.Common.Contract;
using PubSub.Publisher.Abstract;

namespace PubSub.Publisher
{
    internal class Worker : BackgroundService
    {
        private Timer timer;
        private int counter = 0;

        private readonly ILogger<Worker> logger;
        private readonly IBusControl bus;
        private readonly ISendDataService sendDataService;
        private readonly TimerSettings timerSettings;

        public Worker(ILogger<Worker> logger, IBusControl bus, ISendDataService sendDataService, IOptionsMonitor<TimerSettings> timerOptions)
            : base()
        {
            this.logger = logger;
            this.bus = bus;
            this.sendDataService = sendDataService;
            timerSettings = timerOptions.CurrentValue;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(timerSettings.TimerSeconds));

            await bus.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            await bus.StopAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            timer?.Dispose();

            base.Dispose();
        }

        private void DoWork(object state)
        {
            logger.LogInformation("DoWork has been called.");

            IRequestContractData requestContractData = new RequestContractData
            {
                Id = ++counter,
                CorrelationId = Guid.NewGuid(),
                Created = DateTimeOffset.Now
            };

            sendDataService.PostAsync(requestContractData);
        }
    }
}
