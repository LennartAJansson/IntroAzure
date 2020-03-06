using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UsingKeyVault
{
    internal class Worker : BackgroundService
    {
        private Timer timer;

        private readonly ILogger<Worker> logger;
        private readonly TimerSettings timerSettings;
        private readonly AppSecret appSecret;

        public Worker(ILogger<Worker> logger, IOptionsMonitor<TimerSettings> timerOptions, IOptionsMonitor<AppSecret> secretOptions)
        {
            this.logger = logger;
            timerSettings = timerOptions.CurrentValue;
            appSecret = secretOptions.CurrentValue;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(timerSettings.TimerSeconds));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            return Task.CompletedTask;

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            timer?.Dispose();

            base.Dispose();
        }

        private void DoWork(object state) => logger.LogInformation($"DoWork has been called. AppSecret is: {appSecret.MyAppSecret}");
    }
}
