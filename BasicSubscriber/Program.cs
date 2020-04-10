using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace BasicSubscriber
{
    internal class Program
    {
        private static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .Run();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration(config => config.AddExtraConfiguration<Program>())

                .UseSerilog((hostContext, config) => config.ReadFrom.Configuration(hostContext.Configuration))

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    services.Configure<KeyVaultSettings>(options => hostContext.Configuration.GetSection("KeyVault").Bind(options));

                    services.Configure<ServiceBusSettings>(options => hostContext.Configuration.GetSection("ServiceBus").Bind(options));

                    services.AddHostedService<Worker>();
                });
    }
}
