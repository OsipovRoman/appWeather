using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibWeather.Interfaces
{
    /// <summary>
    /// Интерфейс реализующий получение данных о погоде
    /// </summary>
    internal interface IWeatherForecast
    {
        /// <summary>
        /// Данные о текущей погоде в городе
        /// </summary>
        /// <param name="city">Город для которого нужно получить данные</param>
        /// <returns>Данные о текущей погоде</returns>
        IForecastWeatherInCity GetWeatherForecastFromCity(ICity city);
    }
}