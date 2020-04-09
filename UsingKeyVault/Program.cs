
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace UsingKeyVault
{
    //https://docs.microsoft.com/sv-se/cli/azure/keyvault?view=azure-cli-latest
    //https://www.codit.eu/blog/the-danger-of-using-config-addazurekeyvault-in-net-core/?country_sel=be
    //https://stackoverflow.com/questions/55201051/bind-key-vault-settings-to-class
    //https://docs.microsoft.com/en-us/azure/azure-app-configuration/use-key-vault-references-dotnet-core?tabs=cmd%2Ccore2x
    internal class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration(config => config.AddExtraConfiguration())

                .UseSerilog((hostContext, config) =>
                    config.ReadFrom.Configuration(hostContext.Configuration))

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    services.Configure<AppSecret>(options =>
                        hostContext.Configuration.GetSection("AppSecret").Bind(options));

                    services.Configure<TimerSettings>(options =>
                        hostContext.Configuration.GetSection("TimerSettings").Bind(options));

                    services.AddHostedService<Worker>();
                });
    }
}