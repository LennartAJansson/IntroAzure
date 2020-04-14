using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherForecast
{
    //Interface for DI, lets us inject our own faked test implementation of Database or a real one
    public interface IDatabase
    {
        Task SaveData(Forecast weatherForecast);
        Task<IEnumerable<Forecast>> GetDataAsync();
    }
}