using System;
using System.Threading;
using System.Threading.Tasks;

using IntroducingServiceBus.Common.Abstract;
using IntroducingServiceBus.Common.Contract;
using IntroducingServiceBus.Common.Extension;
using IntroducingServiceBus.Sender.Abstract;

using MassTransit;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntroducingServiceBus.Sender
{
    internal class Worker : BackgroundService
    {
        private Timer timer;
        private int counter = 0;

        private readonly ILogger<Worker> logger;
        private readonly IBusControl bus;
        private readonly TimerSettings timerSettings;
        private readonly ISendDataService sendDataService;

        public Worker(ILogger<Worker> logger, IBusControl bus, IOptionsMonitor<TimerSettings> timerOptions, ISendDataService sendDataService)
            : base()
        {
            this.logger = logger;
            this.bus = bus;
            timerSettings = timerOptions.CurrentValue;
            this.sendDataService = sendDataService;
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

        private async void DoWork(object state)
        {
            logger.LogInformation("DoWork has been called.");

            IRequestContractData requestContractData = new RequestContractData
            {
                Id = ++counter,
                CorrelationId = Guid.NewGuid(),
                Created = DateTimeOffset.Now
            };

            var responseContractData = await sendDataService.PostAsync(requestContractData);

            logger.LogInformation($"After a roundtrip out on the queue we received: {responseContractData.AsString()}");
        }
    }
}
