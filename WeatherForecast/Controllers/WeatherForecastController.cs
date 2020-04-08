using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WeatherForecast.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDatabase database;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDatabase database)
        {
            _logger = logger;
            this.database = database;
        }

        [HttpGet]
        public IEnumerable<Forecast> Get()
        {
            var w = database.GetData().Result;
            return w.ToArray();
        }
    }

    public class Database : IDatabase
    {
        private static readonly string[] Summaries = new[]
{
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching", "HotterThanHell"
        };

        //Production version of implementation, typically it will get real data from a real database
        public Task<IEnumerable<Forecast>> GetData()
        {
            return Task.FromResult(Enumerable.Range(1, 1).Select(i => new Forecast
            {
                Date = DateTimeOffset.Now,
                TemperatureC = 10,
                Summary = Summaries[5]
            }));
        }

        public Task SaveData(Forecast forecast)
        {
            return Task.CompletedTask;
        }
    }

    //Interface for DI, lets us inject our own faked test implementation of Database
    public interface IDatabase
    {
        Task SaveData(Forecast weatherForecast);
        Task<IEnumerable<Forecast>> GetData();
    }
}
