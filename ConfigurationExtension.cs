using System;
using System.IO;

namespace Microsoft.Extensions.Configuration
{
    //This file is shared as a link to this project, it's not included physically
    //Its real location is in the solution root
    //It is commonly used to add the json-files generated from the powershell scripts that add the Azure infrastructure
    //It also makes the keyvault and user secrets a part of your configuration
    //At the end of this file you will find model classes for the different configuration sections

    internal static class ConfigurationExtension
    {
        internal static IConfigurationBuilder AddExtraConfiguration<TEntity>(this IConfigurationBuilder configuration)
            where TEntity : class
        {
            var where = Directory.GetCurrentDirectory();
            var keyvaultConfig = $"{Directory.GetCurrentDirectory()}\\..\\keyvault.json";
            //var servicebusConfig = $"{Directory.GetCurrentDirectory()}\\..\\servicebus.json";

            configuration.AddUserSecrets<TEntity>();
            configuration.AddJsonFile(keyvaultConfig, optional: true);
            //configuration.AddJsonFile(servicebusConfig, optional: true);

            var builtConfig = configuration.Build();

            if (TestConfig(builtConfig, "KeyVault:Name", "KeyVault:ClientId", "KeyVault:ClientSecret"))
            {
                configuration.AddAzureKeyVault($"https://{builtConfig["KeyVault:Name"]}.vault.azure.net/",
                    builtConfig["KeyVault:ClientId"],
                    builtConfig["KeyVault:ClientSecret"]);

                //Rebuild a temporary config again including the keyvault
                builtConfig = configuration.Build();
            }
            else
            {
                Console.WriteLine($"{keyvaultConfig} not found, did you run the script SetupKeyVault.ps1?");
                Console.WriteLine("If you already created your keyvault then make sure the keyvault config is included in user secrets or keyvault.json!");
            }

            if (!TestConfig(builtConfig, "ServiceBus:ConnectionString", "ServiceBus:Queue", "ServiceBus:Topic", "ServiceBus:Subscription"))
            {
                Console.WriteLine($"Following information isn't neccessary an error:");
                Console.WriteLine($"Servicebus settings not found, did you run the script SetupServiceBus.ps1?");
                Console.WriteLine("If you are running the basic samples for using Azure Servicebus, they will not work!");
                Console.WriteLine("If you already created your servicebus then make sure the servicebus config is included in user secrets, keyvault or servicebus.json!");
            }

            if (!TestConfig(builtConfig, "EventHub:EventHubConnectionString", "EventHub:EventHubName", "EventHub:BlobConnectionString", "EventHub:BlobContainerName"))
            {
                Console.WriteLine($"Following information isn't neccessary an error:");
                Console.WriteLine($"Eventhub settings not found, did you run the script SetupEventHub.ps1?");
                Console.WriteLine("If you are running the basic samples for using Azure Eventhub, they will not work!");
                Console.WriteLine("If you already created your eventhub then make sure the eventhub config is included in user secrets, keyvault or eventhub.json!");
            }

            return configuration;
        }

        private static bool TestConfig(IConfigurationRoot config, params string[] values)
        {
            //Just throw if value doesn't exist
            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(config[value]))
                    throw new ArgumentException($"Value {value} is not in the configuration! Read the documentation!");
            }

            return true;
        }
    }

    public class KeyVaultSettings
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Url => $"https://{Name}.vault.azure.net/";
    }

    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string Queue { get; set; }
        public string Topic { get; set; }
        public string Subscription { get; set; }
        public string Url
        {
            get
            {
                if (ConnectionString.ToLower().Contains("endpoint=sb://"))
                {
                    string result = ConnectionString.Substring(ConnectionString.IndexOf('=') + 1);
                    result = result.Substring(0, result.IndexOf(';'));
                    return result;
                }
                else return string.Empty;
            }
        }
    }

    public class EventHubSettings
    {
        public string EventHubConnectionString { get; set; }
        public string EventHubName { get; set; }
        public string BlobConnectionString { get; set; }
        public string BlobContainerName { get; set; }
        public string Url
        {
            get
            {
                if (EventHubConnectionString.ToLower().Contains("endpoint=sb://"))
                {
                    string result = EventHubConnectionString.Substring(EventHubConnectionString.IndexOf('=') + 1);
                    result = result.Substring(0, result.IndexOf(';'));
                    return result;
                }
                else return string.Empty;
            }
        }
    }
}
