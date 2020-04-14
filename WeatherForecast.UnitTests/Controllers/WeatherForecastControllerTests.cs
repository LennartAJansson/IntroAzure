using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WeatherForecast.Controllers.Tests
{
    [TestClass()]
    public class WeatherForecastControllerTests
    {
        [TestMethod()]
        public async Task GetTest()
        {
            //ARRANGE
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                    loggingBuilder.AddConsole())
                .AddScoped<WeatherForecastController>()
                .AddScoped<IDatabase, FakeDatabase>()
                .AddSingleton<IConfiguration>(configuration)
                .BuildServiceProvider();

            //ACT
            var wfc = serviceProvider.GetService<WeatherForecastController>();
            var fc = await wfc.GetAsync();

            //ASSERT
            Assert.IsTrue(fc.Count() > 0);
            Assert.AreEqual(fc.First().Summary, "Unknown");
            Assert.AreEqual(fc.First().TemperatureC, 10);
        }
    }

    public class FakeDatabase : IDatabase
    {
        public Task<IEnumerable<Forecast>> GetDataAsync()
        {
            var l = new List<Forecast>();
            var w = new Forecast { Date = DateTime.Now, Summary = "Unknown", TemperatureC = 10 };
            l.Add(w);
            return Task.FromResult(l.Select(x => x)); //LINQ
            //return Task.FromResult(l.Select(SelectAllForecast)); //LINQ
        }

        public Task SaveData(Forecast forecast)
        {
            //Save the weatherForecast
            return Task.CompletedTask;
        }
    }
}