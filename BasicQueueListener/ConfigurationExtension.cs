using System;
using System.IO;

using Microsoft.Extensions.Configuration;

namespace BasicQueueListener
{
    internal static class ConfigurationExtension
    {
        internal static IConfigurationBuilder AddExtraConfiguration(this IConfigurationBuilder configuration)
        {
            var keyvaultConfig = $"{Directory.GetCurrentDirectory()}\\..\\keyvault.json";
            if (File.Exists(keyvaultConfig))
            {
                Console.WriteLine("You could take the content of keyvault.json and include it in your user secret instead of reading it here!!!");
                configuration.AddJsonFile($"{Directory.GetCurrentDirectory()}\\..\\keyvault.json", optional: true);

                var builtConfig = configuration.Build();

                TestConfig(builtConfig, "KeyVault:Name", "KeyVault:ClientId", "KeyVault:ClientSecret");

                configuration.AddAzureKeyVault($"https://{builtConfig["KeyVault:Name"]}.vault.azure.net/",
                    builtConfig["KeyVault:ClientId"],
                    builtConfig["KeyVault:ClientSecret"]);
            }
            else
            {
                Console.WriteLine($"{keyvaultConfig} not found, did you run the script SetupKeyVault.ps1?");
                Console.WriteLine("If you already created your keyvault then make sure the keyvault config is included in user secrets or keyvault.json!");
            }

            configuration.AddUserSecrets<Program>();

            return configuration;
        }

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
