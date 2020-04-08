using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WeatherForecast.IntegrationTests.Controllers
{
    [TestClass]
    public class WeatherForecastControllerTests
    {
        [TestMethod]
        public async Task TestWeatherForecastControllerGetOverHttp()
        {
            //ARRANGE
            using HttpClient client = new HttpClient();

            //ACT
            var response = await client.GetAsync("http://40.127.242.168/weatherforecast");
            var json = await response.Content.ReadAsStringAsync();
            //IEnumerable<WeatherForecast> fc = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(json);
            IEnumerable<Forecast> fc = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Forecast>>(json);

            //ASSERT
            Assert.IsTrue(fc.Count() > 0);
            Assert.AreEqual(fc.First().Summary, "Warm");
            Assert.AreEqual(fc.First().TemperatureC, 10);
        }
    }
}
