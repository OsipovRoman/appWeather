using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibWeather.Interfaces;
using System.Runtime.Serialization;

namespace LibWeather.Classes
{
    /// <summary>
    /// Класс города
    /// </summary>
    [DataContract]
    class City : ICity
    {
        /// <summary>
        /// Широта
        /// </summary>
        [DataMember]
        public double lat { get; set; }
        /// <summary>
        /// Долгода
        /// </summary>
        [DataMember]
        public double lon { get; set; }
        /// <summary>
        /// Название города
        /// </summary>
        [DataMember]
        public string name { get; set; }


        /// <summary>
        /// Сравнение объектов для сортировки
        /// </summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>Если меньше 0, то объект перед текущим, если больше то после, 0 равны</returns>
        public int CompareTo(ICity obj)
        {
            return name.ToUpper().CompareTo(obj.name.ToUpper());
        }

        /// <summary>
        /// Сравнение объектов
        /// </summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>результат сравнения</returns>
        public bool Equals(ICity obj)
        {
            return ((CompareTo(obj) == 0) && (lat == obj.lat) && (lon == obj.lon));
        }
    }
}
