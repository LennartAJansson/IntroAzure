using System;

using MassTransit;
using MassTransit.Azure.ServiceBus.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PubSub.Subscriber1.Service;

using Serilog;

namespace PubSub.Subscriber1
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
                    config.AddUserSecrets<ServiceBusSubscriberConfig>())

                .UseSerilog((hostContext, config) =>
                    config.ReadFrom.Configuration(hostContext.Configuration))

                .ConfigureServices((hostContext, services) =>
                {
                    #region MassTransit related
                    if (string.IsNullOrEmpty(hostContext.Configuration["ServiceBus:ConnectionString"]) ||
                        string.IsNullOrEmpty(hostContext.Configuration["ServiceBus:TopicName"]) ||
                        string.IsNullOrEmpty(hostContext.Configuration["ServiceBus:SubscriptionName"]))
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

                busFactoryConfig.SubscriptionEndpoint(configuration["ServiceBus:SubscriptionName"], configuration["ServiceBus:TopicName"], ep =>
                    ep.ConfigureConsumer<RequestDataConsumer>(provider));
            });

            return azureServiceBus;
        }
    }
}
