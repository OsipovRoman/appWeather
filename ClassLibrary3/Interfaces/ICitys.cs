using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibWeather.Interfaces
{
    /// <summary>
    /// Интерфейс предоставляющий список городов
    /// </summary>
    interface ICitys
    {
        /// <summary>
        /// Список доступных городов
        /// </summary>
        /// <returns></returns>
        Task<List<ICity>> getCitys();
    }
}
