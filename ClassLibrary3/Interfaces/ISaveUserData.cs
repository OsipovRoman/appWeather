using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibWeather.Interfaces
{
    /// <summary>
    /// Интерфейс сохранения и загрузки данных о текущем и исзбранных городах
    /// </summary>
    interface ISaveUserData
    {
        /// <summary>
        /// Сохранить данные о текущем городе пользователя
        /// </summary>
        /// <param name="city">Текущий город пользователя</param>
        /// <returns>Результат записи</returns>
        Task<bool> SaveUserCurrentCity(ICity city);
        /// <summary>
        /// Сохранить список избранных городов пользователя
        /// </summary>
        /// <param name="lCity">Список избранных городов пользователя</param>
        /// <returns>Результат записи</returns>
        Task<bool> SaveUserCitys(List<ICity> lCity);
        /// <summary>
        /// Загрузить данные о текущем городе пользователя
        /// </summary>
        /// <returns>Текущий город пользователя</returns>
        Task<ICity> LoadUserCurrentCity();
        /// <summary>
        /// Загрузить данные о избранных городах пользователя
        /// </summary>
        /// <returns></returns>
        Task<List<ICity>> LoadUserCitys();

    }
}
