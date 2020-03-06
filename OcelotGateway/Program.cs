using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace OcelotGateway
{
    public class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
               .Build()
               .Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false)
                   .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                    config
                        .AddJsonFile("ocelot.json")
                        .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true))

                .ConfigureWebHostDefaults(webBuilder => webBuilder
                        .UseSetting("https_port", config.GetValue<string>("Host:httpsPort"))
                        .UseUrls($"http://*:{config.GetValue<int>("Host:httpPort")};https://*:{config.GetValue<int>("Host:httpsPort")}")
                        .UseStartup<Startup>());
        }
    }
}
