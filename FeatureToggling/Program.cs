using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

namespace FeatureToggling
{
    //https://docs.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core
    //https://docs.microsoft.com/en-us/azure/azure-app-configuration/manage-feature-flags
    static class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddFeatureManagement(hostContext.Configuration.GetSection("MyFeatureFlags"))
                        .AddFeatureFilter<PercentageFilter>();

                    services.AddHostedService<Worker>();
                });
    }
}
