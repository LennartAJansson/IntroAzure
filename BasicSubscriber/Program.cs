using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BasicSubscriber
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
                    config.AddUserSecrets<ServiceBusSubscriberConfig>())

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    services.Configure<ServiceBusSubscriberConfig>(options =>
                        hostContext.Configuration.GetSection("ServiceBus").Bind(options));

                    services.AddHostedService<Worker>();
                });
    }
}
