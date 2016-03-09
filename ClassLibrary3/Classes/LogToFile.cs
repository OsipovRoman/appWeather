using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibWeather.Interfaces;
using Windows.Storage;
using System.Globalization;

namespace LibWeather.Classes
{
    /// <summary>
    /// Запись лога в локальный файл
    /// </summary>
    class LogToFile : ILog
    {
        /// <summary>
        /// Возможность записи в Log
        /// </summary>
        bool CanWriteLog = true;

        /// <summary>
        /// Имя Logфайла
        /// </summary>
        const string NameLogFile = "log.txt";
        
        /// <summary>
        /// Файл для записи лога
        /// </summary>
        StorageFile LogFile;

        /// <summary>
        /// Папка для записи лога
        /// </summary>
        StorageFolder LogFolder = ApplicationData.Current.LocalFolder;

        /// <summary>
        /// Получение файла для записи
        /// </summary>
        async Task<bool> GetLogFile()
        {
            //Попытаться получить файл лога
            StorageFile sf = await LogFolder.GetFileAsync(NameLogFile);
            
            //Если файла нет, то попробовать его создать
            if (sf == null)
                try
                {
                    sf = await LogFolder.CreateFileAsync(NameLogFile);
                }
                catch
                {
                    CanWriteLog = false;
                }

            LogFile = sf;

            return CanWriteLog;
        }

        /// <summary>
        /// Запись сообщения в Log
        /// </summary>
        /// <param name="msg"></param>
        public async void SaveMessage(string msg)
        {
            //Если сообщение пустое то не записываем
            if (String.IsNullOrWhiteSpace(msg)) return;
            //Можно ли записывать в лог
            if (!CanWriteLog) return;
            //Если файл не создан, попробовать создать
            if (LogFile == null)
                //Если не создан, то не записываем
                if (!(await GetLogFile()))
                    return;

            //Добавть текст в файл
            await FileIO.AppendTextAsync(LogFile, DateTime.Now.ToString("G", CultureInfo.InvariantCulture) + ": " + msg);
        }
    }
}
