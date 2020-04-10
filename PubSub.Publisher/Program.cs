using System;

using MassTransit;
using MassTransit.Azure.ServiceBus.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PubSub.Common.Abstract;
using PubSub.Publisher.Abstract;
using PubSub.Publisher.Process;

using Serilog;

namespace PubSub.Publisher
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
                    if (string.IsNullOrEmpty(hostContext.Configuration["ServiceBus:ConnectionString"]) ||
                        string.IsNullOrEmpty(hostContext.Configuration["ServiceBus:Topic"]))
                    {
                        throw new ArgumentException("You need to provide parameters for the service bus in your secrets file");
                    }

                    var azureServiceBus = Bus.Factory.CreateUsingAzureServiceBus(busFactoryConfig =>
                    {
                        var host = busFactoryConfig.Host(hostContext.Configuration["ServiceBus:ConnectionString"]);

                        busFactoryConfig.Message<IRequestContractData>(topology => topology.SetEntityName(hostContext.Configuration["ServiceBus:Topic"]));
                    });

                    services.AddMassTransit(massTransit => massTransit.AddBus(provider => azureServiceBus));
                    #endregion

                    services.AddTransient<ISendDataService, SendDataService>();

                    services.AddHostedService<Worker>();
                });
    }
}
