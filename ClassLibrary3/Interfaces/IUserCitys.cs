using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibWeather.Classes;

namespace LibWeather.Interfaces
{
    interface IUserCitys
    {
        /// <summary>
        /// Получить список избранных городов пользователя
        /// </summary>
        /// <returns>Список избрайнных городов</returns>
        Task<List<ICity>> GetSelectedCitys();
        /// <summary>
        /// Получить текущий город пользователя
        /// </summary>
        /// <returns>Текущий город пользователя</returns>
        Task<ICity> GetUserCity();
        /// <summary>
        /// Добавить город в список
        /// </summary>
        Task<bool> AddCityToSelected(ICity city);
        /// <summary>
        /// Удалить город из списка изранных
        /// </summary>
        /// <param name="city"></param>
        Task<bool> RemoveCityFromSelected(ICity city);
        /// <summary>
        /// Установка нового текущего города пользователя
        /// </summary>
        /// <param name="city"></param>
        Task<bool> SetUserCity(ICity city);
        /// <summary>
        /// Список прогнозов погоды для городов пользователя
        /// </summary>
        List<IForecastWeatherInCity> LForecastForUserCitys { get; }
        /// <summary>
        /// События обновление прогозов погоды
        /// </summary>
        event EventHandler<WeatherForecastEventArgs> UpdateForecast;
        /// <summary>
        /// Возвращает список доступных городов для выбора, за искючением текущего и избранных на данный момент
        /// </summary>
        /// <returns>список доступных городов для выбора, за искючением текущего и избранных на данный момент</returns>
        Task<List<ICity>> getListUserForAddNewCityInFavorites();
        /// <summary>
        /// Возвращает список доступных городов для выбора текущего города
        /// </summary>
        /// <returns>список доступных городов для выбора</returns>
        Task<List<ICity>> getListUserForSelectUserCity();
    }
}