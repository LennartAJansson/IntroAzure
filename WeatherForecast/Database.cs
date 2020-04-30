using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherForecast
{
    //Production version of implementation, typically it will get real data from a real database
    public class Database : IDatabase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching", "HotterThanHell"
        };

        public Task<IEnumerable<Forecast>> GetDataAsync()
        {
            return Task.FromResult(Enumerable.Range(1, 1).Select(i => new Forecast
            {
                Date = DateTimeOffset.Now,
                TemperatureC = 20,
                Summary = Summaries[6]
            }));
        }

        public Task SaveData(Forecast forecast)
        {
            return Task.CompletedTask;
        }
    }
}