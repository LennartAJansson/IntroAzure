﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BasicPublisher
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
                    config.AddUserSecrets<ServiceBusPublisherConfig>())

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    services.Configure<TimerSettings>(options =>
                        hostContext.Configuration.GetSection("TimerSettings").Bind(options));

                    services.Configure<ServiceBusPublisherConfig>(options =>
                        hostContext.Configuration.GetSection("ServiceBus").Bind(options));

                    services.AddHostedService<Worker>();
                });
    }
}
