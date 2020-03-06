using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BasicQueueListener
{
    static class Program
    {
        private static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .Run();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)

                .ConfigureAppConfiguration(config =>
                    config.AddUserSecrets<ServiceBusQueueConfig>())

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    services.Configure<ServiceBusQueueConfig>(options =>
                        hostContext.Configuration.GetSection("ServiceBus").Bind(options));

                    services.AddHostedService<Worker>();
                });
    }
}
