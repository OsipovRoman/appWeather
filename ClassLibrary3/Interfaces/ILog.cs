using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibWeather.Interfaces
{
    /// <summary>
    /// Интерфейс записи лога
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Записать сообщение
        /// </summary>
        /// <param name="msg">Текст сообщения</param>
        void SaveMessage(string msg);
    }
}
