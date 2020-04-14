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
        private readonly ILogger<WeatherForecastController> logger;
        private readonly IDatabase database;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDatabase database)
        {
            this.logger = logger;
            this.logger.LogInformation("Constructing controller");
            this.database = database;
        }

        [HttpGet]
        public async Task<IEnumerable<Forecast>> GetAsync()
        {
            logger.LogInformation("Executing Get in controller");
            var w = await database.GetDataAsync();
            return w.ToArray();
        }
    }
}
