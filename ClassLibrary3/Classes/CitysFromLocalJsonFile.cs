using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using LibWeather.Interfaces;

namespace LibWeather.Classes
{
    /// <summary>
    /// Получение списка городов из локального файла
    /// </summary>
    class CitysFromLocalJsonFile : ICitys
    {
        ILog Log;

        #region константы названий полей в JSON файле
        /// <summary>
        /// Широта
        /// </summary>
        const string lat = "coord.lat";
        /// <summary>
        /// Долгота
        /// </summary>
        const string lon = "coord.lon";
        /// <summary>
        /// Название города
        /// </summary>
        const string name = "name";
        #endregion

        #region константы названий файлов для записи
        /// <summary>
        /// Название файла хранящего список городов
        /// </summary>
        readonly Uri  UriCitys = new Uri("ms-appx:///Res/city.list.json");
        #endregion

        /// <summary>
        /// Загрузка JSON объекта из файла
        /// </summary>
        /// <returns>Считанный JSON объект</returns>
        async Task<JsonArray> LoadJsonFile()
        {
            JsonArray jo = new JsonArray();
            //Попытаться получить файл c JSON объектом
            StorageFile sf = await StorageFile.GetFileFromApplicationUriAsync(UriCitys);

            //Если файла нет, то попробовать его создать
            if (sf != null)
            {
                string txtJSON = string.Empty;
                bool FlagTrueRead = true;
                try
                {
                    txtJSON = await FileIO.ReadTextAsync(sf);
                }
                catch (Exception e)
                {
                    Log.SaveMessage("CitysFromLocalJsonFile: При считывании данных из файла ресурсов произошла ошибка. Текст ошибки: "
                        + e.Message);
                    FlagTrueRead = false;
                }
                if (FlagTrueRead)
                    if (!JsonArray.TryParse(txtJSON, out jo))
                        jo = null;
            }
            return jo;
        }

        /// <summary>
        /// Преобразование JSONObject в ICity
        /// </summary>
        /// <param name="jo">Входящий jo</param>
        /// <returns>Полученный объект ICity</returns>
        ICity JsonToICity(JsonObject jo)
        {
            ICity city = null;
            if (jo != null)
            {
                city = new City()
                {
                    name = jo.GetNamedString(name),
                    lat = jo.GetNamedNumber(lat),
                    lon = jo.GetNamedNumber(lon)
                };
            }
            return city;
        }

        /// <summary>
        /// Получение списка избранных городов пользователя
        /// </summary>
        /// <returns>Список избранных городов</returns>
        public async Task<List<ICity>> LoadCitys()
        {

            JsonArray jo = await LoadJsonFile();
            List<ICity> lc = new List<ICity>();
            foreach (JsonObject jv in jo)
                lc.Add(JsonToICity(jv));
            return lc;
        }

        public async Task<List<ICity>> getCitys()
        {
            return await LoadCitys();
        }

        public CitysFromLocalJsonFile(ILog log)
        {
            Log = log;
        }
    }
}
