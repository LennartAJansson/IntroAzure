using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace FeatureToggling
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IFeatureManager featureManager;

        public Worker(ILogger<Worker> logger, IFeatureManager featureManager)
        {
            this.logger = logger;
            this.featureManager = featureManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                if (await featureManager.IsEnabledAsync(nameof(MyFeatureFlags.FeatureA)))
                {
                    // Run the following code
                }
                //Use attribute in controller: [FeatureGate(MyFeatureFlags.FeatureA)]
                //Use a <feature> tag to render content based on whether a feature flag is enabled: <feature name="FeatureA">
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
