using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibWeather.Interfaces
{
    /// <summary>
    /// Интерфейс данных о городе
    /// </summary>
    public interface ICity: IComparable<ICity>, IEquatable<ICity>
    {
        /// <summary>
        /// Широта
        /// </summary>
        double lat { get; set; }
        /// <summary>
        /// Долгота
        /// </summary>
        double lon { get; set; }
        /// <summary>
        /// Название города
        /// </summary>
        string name { get; set; }
    }
}