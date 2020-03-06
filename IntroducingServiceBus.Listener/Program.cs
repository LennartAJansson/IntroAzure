using System;

using IntroducingServiceBus.Listener.Service;

using MassTransit;
using MassTransit.Azure.ServiceBus.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace IntroducingServiceBus.Listener
{
    static class Program
    {
        private static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .Run();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration(config =>
                    config.AddUserSecrets<ServiceBusQueueConfig>())

                .UseSerilog((hostContext, config) =>
                    config.ReadFrom.Configuration(hostContext.Configuration))

                .ConfigureServices((hostContext, services) =>
                {
                    #region MassTransit related
                    if (string.IsNullOrEmpty(hostContext.Configuration["ServiceBus:ConnectionString"]) ||
                        string.IsNullOrEmpty(hostContext.Configuration["ServiceBus:QueueName"]))
                    {
                        throw new ArgumentException("You need to provide parameters for the service bus in your secrets file");
                    }

                    services.AddMassTransit(massTransit =>
                    {
                        massTransit.AddConsumer<RequestDataConsumer>();

                        massTransit.AddBus(ConfigureBus);
                    });
                    #endregion

                    services.AddHostedService<Worker>();
                });

        private static IBusControl ConfigureBus(IServiceProvider provider)
        {
            var configuration = provider.GetService<IConfiguration>();

            var azureServiceBus = Bus.Factory.CreateUsingAzureServiceBus(busFactoryConfig =>
            {
                IServiceBusHost host = busFactoryConfig.Host(configuration["ServiceBus:ConnectionString"]);

                busFactoryConfig.ReceiveEndpoint(configuration["ServiceBus:QueueName"], ep =>
                    ep.ConfigureConsumer<RequestDataConsumer>(provider));
            });

            return azureServiceBus;
        }
    }
}
