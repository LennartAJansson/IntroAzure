using System;
using System.IO;

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
    internal static class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var keyvaultConfig = $"{Directory.GetCurrentDirectory()}\\..\\keyvault.json";
                    if (File.Exists(keyvaultConfig))
                    {
                        Console.WriteLine("You could take the content of keyvault.json and include it in your user secret instead of reading it here!!!");
                        config.AddJsonFile($"{Directory.GetCurrentDirectory()}\\..\\keyvault.json", optional: true);
                    }
                    else
                    {
                        Console.WriteLine($"{keyvaultConfig} not found, did you run the script SetupKeyVault.ps1?");
                        Console.WriteLine("If you already created your keyvault then make sure the config is included in the user secrets!");
                    }
                    Console.WriteLine("Try to enable and disable the line below this statement to see the difference in reading from secret and keyvault");
                    //Toggle the next line to see difference between reading from UserSecret and KeyVault
                    if (hostContext.HostingEnvironment.IsProduction())
                    {
                        var builtConfig = config.Build();

                        TestConfig(builtConfig, "KeyVault:Name", "KeyVault:ClientId", "KeyVault:ClientSecret");

                        config.AddAzureKeyVault($"https://{builtConfig["KeyVault:Name"]}.vault.azure.net/",
                            builtConfig["KeyVault:ClientId"],
                            builtConfig["KeyVault:ClientSecret"]);
                    }
                })

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

        private static void TestConfig(IConfigurationRoot config, params string[] values)
        {
            //Just throw if value doesn't exist
            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(config[value]))
                    throw new ArgumentException($"Value {value} is not in the configuration! Read the documentation!");
            }
        }
    }
}