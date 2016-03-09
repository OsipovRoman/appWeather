using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibWeather.Interfaces;
using System.IO;
using System.Net;
using Windows.Networking.Connectivity;
using Windows.Data.Json;
using System.Threading;
using System.Runtime.Serialization.Json;

namespace LibWeather.Classes
{

    /// <summary>
    /// Класс для хранения выполнения запроса
    /// </summary>
    class RequestState
    {
        //const int BUFFER_SIZE = 1024;
        //public StringBuilder requestData;
        //public byte[] bufferRead;
        public WebRequest request;
        //public WebResponse response;
        public Stream responseStream;
        public RequestState()
        {
            //bufferRead = new byte[BUFFER_SIZE];
            //requestData = new StringBuilder("");
            request = null;
            responseStream = null;
        }
    }

    /// <summary>
    /// Получение прогноза погоды от Openweathermap
    /// </summary>
    class WeatherForecastFromOpenweathermap : IWeatherForecast
    {
        /// <summary>
        /// Ключ доступа к openweathermap
        /// </summary>
        const string AppId = "4678f7f3f74c98e91fdabb60e02fb302";
        /// <summary>
        /// Адрес запроса погоды
        /// </summary>
        const string Site = "http://api.openweathermap.org/data/2.5/";
        /// <summary>
        /// Адрес запроса изображений
        /// </summary>
        const string SiteImg = "http://openweathermap.org/img/w/";
        /// <summary>
        /// Объект записи Лога
        /// </summary>
        ILog Log;
        /// <summary>
        /// Начало Эпохи Unix
        /// </summary>
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0,
                                                          DateTimeKind.Utc);



        /// <summary>
        /// Преобразование милисекунд эпохи Unix в DateTime
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns>Дата сконвертированная в DateTime</returns>
        static DateTime UtcLongToDateTime(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }

        /// <summary>
        /// Заполнение объекта WeatherData из JSON объекта
        /// </summary>
        /// <param name="JsonO">Входящий JSON</param>
        /// <returns>Заполненный объект WeatherData или null в случае если при конверации возницли ошибки</returns>
        WeatherData JsonToWeatherData(JsonObject JsonO)
        {
            WeatherData WD;
            try
            {
                WD = new WeatherData()
                {
                    DTWeather = UtcLongToDateTime(Convert.ToInt64(JsonO.GetNamedString("dt"))),
                    Temp = Convert.ToSingle(JsonO.GetNamedString("main.temp")),
                    Hamidity = Convert.ToSingle(JsonO.GetNamedString("main.humidity")),
                    Pressure = Convert.ToSingle(JsonO.GetNamedString("main.pressure")),
                    Clouds = Convert.ToSingle(JsonO.GetNamedString("clouds.all")),
                    Description = JsonO.GetNamedString("weather.description"),
                    wind_speed = Convert.ToSingle(JsonO.GetNamedString("wind.speed")),
                    wind_deg = Convert.ToSingle(JsonO.GetNamedString("wind.deg")),
                    ImgUrl = SiteImg + JsonO.GetNamedString("weather.icon")
                };
            }
            catch
            {
                Log.SaveMessage("GetWeatherForecastFromCity: Ошибка конвертации данных погоды полученной от сервера.");
                //Если при разборе входящего объекта возникли исключения, то возвращается null
                WD = null;
            }
            return WD;
        }

        ManualResetEvent allDone = new ManualResetEvent(false);

        /// <summary>
        /// Получение потока от сервера
        /// </summary>
        /// <param name="strRequest">текст запроса</param>
        /// <returns></returns>
        String GetStreamFromServer(String strRequest)
        {
            //Проверка наличия интеренет подключения
            if (NetworkInformation.GetInternetConnectionProfile() == null)
            {
                Log.SaveMessage("GetWeatherForecastFromCity: отсутсвует интерент подключение. Запрос не отправлялся.");
                return null;
            }

            String strm = null;
            //Текст запроса к серверу
            try
            {
                //Получение потока от сервера
                RequestState myRequestState = new RequestState()
                {
                    request = WebRequest.Create(Site + strRequest)
                };

                IAsyncResult asyncResult = myRequestState.request.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

                //ожидаем обработки запроса
                allDone.WaitOne();
                StreamReader stream = new StreamReader(myRequestState.responseStream, Encoding.UTF8);
                stream.BaseStream.Position = 0;
                strm = stream.ReadToEnd();
            }
            catch (Exception e)
            {
                //Не получен ответ от сервера
                Log.SaveMessage("GetWeatherForecastFromCity: При обращении к серверу(запрос текущих данных "
                    + strRequest + ") произошла ошибка. Текст ошибки: " + e.Message);
                strm = null;
            }
            return strm;
        }

        private void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                myRequestState.responseStream = myRequestState.request.EndGetResponse(asynchronousResult).GetResponseStream();
                //Сигнализируем об обработке запроса
                allDone.Set();
            }
            catch (Exception e)
            {
                Log.SaveMessage("При получении данных произошла ошибка. Текст ошибки: " + e.Message);
            }
        }
        /// <summary>
        /// Получение прогноза погоды
        /// </summary>
        /// <param name="city">Город для которого нужны данные</param>
        /// <returns>Данные о погоде в городе</returns>
        public IForecastWeatherInCity GetWeatherForecastFromCity(ICity city)
        {
            ForecastWeatherInCity wfd = null;

            if (city == null)
            {
                Log.SaveMessage("GetWeatherForecastFromCity: передан null");
                return null;
            }

            //Получение текущей погоды
            String strmWeather = GetStreamFromServer("weather?lat=" + city.lat + "&lon=" + city.lon + "&AppId=" + AppId);

            if (strmWeather != null)
            {
                //Получение прогноза
                String strmForecast = GetStreamFromServer("forecast?lat=" + city.lat + "&lon=" + city.lon + "&AppId=" + AppId);
                if (strmForecast != null)
                {
                    JsonObject JsonWeather = new JsonObject();
                    JsonObject JsonForecast = new JsonObject();

                    //Флаг правильного парсинга полученных объектов от сервера
                    bool FlagTrueJSONObject = true;
                    if (!JsonObject.TryParse(strmWeather, out JsonWeather))
                    {
                        Log.SaveMessage("Объект(текущая погода) полученный от сервера не является JSON");
                        FlagTrueJSONObject = false;
                    }
                    if (!JsonObject.TryParse(strmForecast, out JsonForecast))
                    {
                        Log.SaveMessage("Объект(прогноз) полученный от сервера не является JSON");
                        FlagTrueJSONObject = false;
                    }

                    //Если оба объекта распарсились без ошибок то пробуем заполнить данными из них объект для возврата
                    if (FlagTrueJSONObject)
                    {
                        List<IWeatherData> lWD = new List<IWeatherData>();
                        WeatherData CurrWD = JsonToWeatherData(JsonWeather);
                        //При разбор данных о текущей погоде произошел без ошибок, произвести разбор списка прогнозов
                        if (CurrWD != null)
                        {
                            foreach (JsonObject jo in JsonForecast.GetNamedArray("list"))
                            {
                                WeatherData WDitem = JsonToWeatherData(JsonForecast);
                                if (WDitem != null)
                                    lWD.Add(WDitem);
                            }
                            //Если прогнозы были разобранны без ошибок, то сформировать объект для возврата
                            if (lWD.Count > 0)
                                wfd = new ForecastWeatherInCity()
                                {
                                    City = city,
                                    CurrentWeather = CurrWD,
                                    lWeather = lWD
                                };
                        }
                    }
                }
            }
            return wfd;
        }

        public WeatherForecastFromOpenweathermap(ILog log)
        {
            Log = log;
        }
    }
}
