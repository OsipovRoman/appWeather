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
    /// Сохраняет и загружает данные о городах пользователя в RoamingFolder
    /// </summary>
    class SaveUserDataToFileInRoamingFolder : ISaveUserData
    {

        ILog Log;

        #region константы названий полей в JSON файле
        /// <summary>
        /// Широта
        /// </summary>
        const string lat = "lat";
        /// <summary>
        /// Долгота
        /// </summary>
        const string lon = "lon";
        /// <summary>
        /// Название города
        /// </summary>
        const string name = "name";
        /// <summary>
        /// список городов
        /// </summary>
        const string LCity = "lCity";
        #endregion

        #region константы названий файлов для записи
        /// <summary>
        /// Название файла хранящего список городов пользователя
        /// </summary>
        const string UserCitysFileName = "UserCitys.json";
        /// <summary>
        /// Название файла хранящего текущий город пользователя
        /// </summary>
        const string UserCurrentCitysFileName = "UserCurrentCity.json";
        #endregion

        /// <summary>
        /// Папка для записи
        /// </summary>
        StorageFolder FileFolder = ApplicationData.Current.RoamingFolder;

        /// <summary>
        /// Загрузка JSON объекта из файла
        /// </summary>
        /// <param name="fname">Имя файла</param>
        /// <returns>Считанный JSON объект</returns>
        async Task<JsonObject> LoadJsonFile(string fname)
        {
            JsonObject jo=new JsonObject();
            //Попытаться получить файл c JSON объектом
            StorageFile sf = await FileFolder.GetFileAsync(fname);

            //Если файла нет, то попробовать его создать и заполняем исходными данными
            if (sf==null)
            {
                List<ICity> lc = new List<ICity>()
                {
                    new City() { name="London", lon=-0.12574,lat=51.50853 },
                    new City() { name="New York",lon=-75.499901,lat=43.000351 },
                    new City() { name="City of Sydney", lon=151.208435, lat=-33.867779 }
                };

                //Если запись прошла удачно пробуем получить файл ещё раз
                if (await SaveUserCitys(lc))
                    sf = await FileFolder.GetFileAsync(fname);
            }

            //Если объект файла получен, то считываем его содержимое и преобразуем в Json
            if (sf != null)
            {
                string txtJSON=string.Empty;
                bool FlagTrueRead = true;
                try
                {
                    txtJSON = await FileIO.ReadTextAsync(sf);
                }
                catch(Exception e)
                {
                    Log.SaveMessage("SaveUserDataToFileInRoamingFolder: При считывании данных из файла "
                        + fname + " произошла ошибка. Текст ошибки: " + e.Message);
                    FlagTrueRead = false;
                }
                if (FlagTrueRead)
                    if (!JsonObject.TryParse(txtJSON, out jo))
                        jo = null;
            }
            
            return jo;
        }

        /// <summary>
        /// Сохранение Json объекта в файл
        /// </summary>
        /// <param name="fname">Название файла</param>
        /// <param name="jo">Объект реализующий интерфайс IJsonValue</param>
        /// <returns>Результат сохранения</returns>
        async Task<bool> SaveJsonFile(string fname, JsonObject jo)
        {
            StorageFile sf;
            //Пересоздаю файл для записи данных
            try
            {
                sf = await FileFolder.CreateFileAsync(fname, CreationCollisionOption.ReplaceExisting);

            }
            catch (Exception e)
            {
                Log.SaveMessage("SaveUserCitys: Неудалось создать файл для сохранения списка выбранных городов пользователя. Текст ошибки: " + e.Message);
                sf = null;
            }

            //Запись полученных данных в файл
            if (sf != null)
            {
                try
                {
                    await FileIO.WriteTextAsync(sf, jo.Stringify());
                }
                catch (Exception e)
                {
                    Log.SaveMessage("SaveUserCitys: Неудалось сохранить список выбранных городов пользователя. Текст ошибки: " + e.Message);
                    sf = null;
                }
            }

            //Результат записи. В случае успешной записи sf не является null
            if (sf == null)
                return false;
            else
                return true;
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
        /// Преобразование Icity в JsonObject
        /// </summary>
        /// <param name="city">Входящий объект ICity</param>
        /// <returns>Сгенерированный JsonObject </returns>
        JsonObject ICityToJson(ICity city)
        {
            JsonObject jo = new JsonObject();
            if (city != null)
            {
                jo.Add(name, JsonValue.CreateStringValue(city.name));
                jo.Add(lat, JsonValue.CreateNumberValue(city.lat));
                jo.Add(lon, JsonValue.CreateNumberValue(city.lon));
            }
            return jo;
        }

        /// <summary>
        /// Получение списка избранных городов пользователя
        /// </summary>
        /// <returns>Список избранных городов</returns>
        public async Task<List<ICity>> LoadUserCitys()
        {
            
            JsonObject jo = await LoadJsonFile(UserCitysFileName);
            JsonArray ja = jo.GetNamedArray(LCity);
            List<ICity> lc = new List<ICity>();
            foreach(JsonObject jv in ja)
                lc.Add(JsonToICity(jv));
            return lc;
        }

        /// <summary>
        /// Получение текущего города пользователя
        /// </summary>
        /// <returns>Текущий город пользователя</returns>
        public async Task<ICity> LoadUserCurrentCity()
        {
            JsonObject jo = await LoadJsonFile(UserCurrentCitysFileName);
            return JsonToICity(jo);
        }

        /// <summary>
        /// Запись файла с городами пользователя
        /// </summary>
        /// <param name="lCity">Список городов для сохранения</param>
        /// <returns>результат записи</returns>
        public async Task<bool> SaveUserCitys(List<ICity> lCity)
        {
            //Серилизация данных списка в JSON
            JsonArray ja = new JsonArray();
            if (lCity != null)
            {
                foreach (ICity ct in lCity)
                    ja.Add(ICityToJson(ct));
            }
            JsonObject jo = new JsonObject();
            jo.Add(LCity, ja);
            return await SaveJsonFile(UserCitysFileName, jo);
        }

        /// <summary>
        /// Запись файла с текущим городом пользователя
        /// </summary>
        /// <param name="city">Текущий город пользователя</param>
        /// <returns>результат записи</returns>
        public async Task<bool> SaveUserCurrentCity(ICity city)
        {
            return await SaveJsonFile(UserCurrentCitysFileName, ICityToJson(city));
        }

        public SaveUserDataToFileInRoamingFolder(ILog log)
        {
            Log = log;
        }
    }
}
