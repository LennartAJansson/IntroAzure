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

                    var azureServiceBus = Bus.Factory.CreateUsingAzureServiceBus(busFactoryConfig =>
                    {
                        var host = busFactoryConfig.Host(hostContext.Configuration["ServiceBus:ConnectionString"]);

                        busFactoryConfig.Message<IRequestContractData>(topology =>
                            topology.SetEntityName(hostContext.Configuration["ServiceBus:QueueName"]));
                    });

                    services.AddMassTransit(massTransit =>
                        massTransit.AddBus(provider => azureServiceBus));
                    #endregion

                    services.AddOptions();

                    services.Configure<ServiceBusQueueConfig>(options =>
                        hostContext.Configuration.GetSection("ServiceBus").Bind(options));

                    services.Configure<TimerSettings>(options =>
                        hostContext.Configuration.GetSection("TimerSettings").Bind(options));

                    services.AddTransient<ISendDataService, SendDataService>();

                    services.AddHostedService<Worker>();
                });
    }
}
