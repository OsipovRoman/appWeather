using System;

namespace LibWeather.Interfaces
{
    /// <summary>
    /// Погодные данные
    /// </summary>
    public interface IWeatherData
    {
        /// <summary>
        /// Время расчета данных
        /// </summary>
        DateTime DTWeather { get; set; }
        /// <summary>
        /// Температура, К
        /// </summary>
        double Temp { get; set; }
        /// <summary>
        /// Облачность, %
        /// </summary>
        double Clouds { get; set; }
        /// <summary>
        /// Влажность, %
        /// </summary>
        double Hamidity { get; set; }
        /// <summary>
        /// Давление, МПа
        /// </summary>
        double Pressure { get; set; }
        /// <summary>
        /// Скорость ветра, м/сек
        /// </summary>
        double wind_speed { get; set; }
        /// <summary>
        /// Направление ветра, град
        /// </summary>
        double wind_deg { get; set; }
        /// <summary>
        /// Описание погоды
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// Изображение погоды
        /// </summary>
        byte[] Img { get; set; }
        /// <summary>
        /// Адрес изображения
        /// </summary>
        string ImgUrl { get; set; }
    }
}