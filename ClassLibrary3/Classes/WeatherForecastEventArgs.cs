using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibWeather.Interfaces;

namespace LibWeather.Classes
{
    class WeatherForecastEventArgs: EventArgs
    {
        /// <summary>
        /// Список прогнозов погоды
        /// </summary>
        public readonly List<IForecastWeatherInCity> lForecastWeather;

        public WeatherForecastEventArgs(List<IForecastWeatherInCity> lFW)
        {
            lForecastWeather = lFW;
        }
    }
}
