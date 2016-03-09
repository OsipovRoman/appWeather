using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibWeather.Interfaces;

namespace LibWeather.Classes
{
    class WeatherData : IWeatherData
    {
        /// <summary>
        /// Время расчета данных
        /// </summary>
        public DateTime DTWeather { get; set; }
        /// <summary>
        /// Температура, К
        /// </summary>
        public double Temp { get; set; }
        /// <summary>
        /// Облачность, %
        /// </summary>
        public double Clouds { get; set; }
        /// <summary>
        /// Влажность, %
        /// </summary>
        public double Hamidity { get; set; }
        /// <summary>
        /// Давление, МПа
        /// </summary>
        public double Pressure { get; set; }
        /// <summary>
        /// Скорость ветра, м/сек
        /// </summary>
        public double wind_speed { get; set; }
        /// <summary>
        /// Направление ветра, град
        /// </summary>
        public double wind_deg { get; set; }
        /// <summary>
        /// Описание погоды
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Изображение погоды
        /// </summary>
        public Byte[] Img { get; set; }
        /// <summary>
        /// Ссылка на изображение в сети
        /// </summary>
        public String ImgUrl { get; set; }
    }
}
