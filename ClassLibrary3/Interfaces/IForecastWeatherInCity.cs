using System;
using System.Collections.Generic;
using LibWeather.Interfaces;

namespace LibWeather.Interfaces
{
    /// <summary>
    /// Данные о погоде
    /// </summary>
    interface IForecastWeatherInCity
    {
        /// <summary>
        /// Город для которого хранятся данные
        /// </summary>
        ICity City { get; set; }
        /// <summary>
        /// Текущие погодные условия
        /// </summary>
        IWeatherData CurrentWeather { get; set; }
        /// <summary>
        /// Прогнозы погоды
        /// </summary>
        List<IWeatherData> lWeather { get; set; }
    }
}