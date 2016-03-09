using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibWeather.Interfaces;

namespace LibWeather.Classes
{
    /// <summary>
    /// Объект данных о погоде
    /// </summary>
    class ForecastWeatherInCity : IForecastWeatherInCity
    {
        /// <summary>
        /// Город для которого ведется прогноз погоды
        /// </summary>
        public ICity City { get; set; }
        /// <summary>
        /// Текущие погодные условия
        /// </summary>
        public IWeatherData CurrentWeather { get; set; }
        /// <summary>
        /// Прогнозы погоды
        /// </summary>
        public List<IWeatherData> lWeather { get; set; }
    }
}
