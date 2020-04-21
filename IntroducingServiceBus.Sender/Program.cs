using System;

using IntroducingServiceBus.Common.Abstract;
using IntroducingServiceBus.Sender.Abstract;
using IntroducingServiceBus.Sender.Service;

using MassTransit;
using MassTransit.Azure.ServiceBus.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace IntroducingServiceBus.Sender
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

                    services.Configure<TimerSettings>(options => hostContext.Configuration.GetSection("TimerSettings").Bind(options));

                    #region MassTransit related
                    services.AddMassTransit(massTransit => massTransit.AddBus(ConfigureBus));
                    #endregion

                    services.AddTransient<ISendDataService, SendDataService>();

                    services.AddHostedService<Worker>();
                });

        private static IBusControl ConfigureBus(IServiceProvider provider)
        {
            var configuration = provider.GetService<IConfiguration>();

            var azureServiceBus = Bus.Factory.CreateUsingAzureServiceBus(busFactoryConfig =>
            {
                var host = busFactoryConfig.Host(configuration["ServiceBus:ConnectionString"]);

                busFactoryConfig.Message<IRequestContractData>(topology => topology.SetEntityName(configuration["ServiceBus:Queue"]));
            });

            return azureServiceBus;
        }
    }
}
