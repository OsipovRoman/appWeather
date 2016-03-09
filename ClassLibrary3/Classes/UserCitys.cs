using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using LibWeather.Interfaces;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Data.Json;
using Windows.System.Threading;
using Windows.Devices.Geolocation;

namespace LibWeather.Classes
{
    class UserCitys : IUserCitys
    {
        ICitys Citys;
        IWeatherForecast wf;

        /// <summary>
        /// период опроса сервера
        /// </summary>
        const int period = 5;

        /// <summary>
        /// Таймер для периодического опроса прогноза погоды
        /// </summary>
        ThreadPoolTimer tpt;

        /// <summary>
        /// Условный город текущего положения
        /// </summary>
        static readonly ICity CurrentCity = new City()
        {
            name = "Current position",
            lat = 0,
            lon = 0
        };

        /// <summary>
        /// Город по умолчанию 
        /// на случай отсутствия выбора своего города пользователем
        /// и невозможности определить позицию
        /// </summary>
        static readonly ICity DefaultCity = new City()
        {
            name = "Moskva",
            lat = 55.761665,
            lon = 37.606667
        };

        ICity CurrentPosition = DefaultCity;

        public event EventHandler<WeatherForecastEventArgs> UpdateForecast;

        /// <summary>
        /// Объект отвечающий за сохранение списков
        /// </summary>
        ISaveUserData Sud;

        /// <summary>
        /// Флаг инициализации города пользователя
        /// </summary>
        bool FlagInitUserCity = false;
        /// <summary>
        /// Флаг инициализации списка избранных городов
        /// </summary>
        bool FlagInitSelectedCitys = false;

        /// <summary>
        /// Список городов пользователя
        /// </summary>
        private List<ICity> selectedCitys = new List<ICity>();

        /// <summary>
        /// Месторасположение пользователя
        /// </summary>
        private ICity userCity = null;

        /// <summary>
        /// Предоставляет доступ к текущему положению
        /// </summary>
        Geolocator gLoсator = new Geolocator()
        {
            //Точность сигнала стандартная
            DesiredAccuracy = PositionAccuracy.Default,
            //При изменении положения более 10 км вызвать событие изменения положения
            MovementThreshold = 10000
        };

        private List<IForecastWeatherInCity> lForecastForUserCitys = new List<IForecastWeatherInCity>();
        /// <summary>
        /// Список прогнозов погоды для городов пользователя
        /// </summary>
        public List<IForecastWeatherInCity> LForecastForUserCitys
        { get { return lForecastForUserCitys; } }

        /// <summary>
        /// Добавить город в выбранные
        /// </summary>
        /// <param name="city">город для добавления</param>
        public async Task<bool> AddCityToSelected(ICity city)
        {
            //Если город отсутвует или данный город является текущим городом пользователя
            if ((city == null) && (city.Equals(userCity))) 
                return false;
            bool ret = true;
            //Добавляем список в город только при его отсутствии в списке
            if (selectedCitys.IndexOf(city) < 0)
            {
                selectedCitys.Add(city);
                //Сохраняем текущий список в файл
                if (!(await Sud.SaveUserCitys(selectedCitys.ToList<ICity>())))
                {
                    //Если при сохранении файла произошли ошибки удалить город из списка
                    selectedCitys.Remove(city);
                    ret = false;
                }
            }
            return ret;
        }

        /// <summary>
        /// Удаление города из выбранных
        /// </summary>
        /// <param name="city">Город который необходимо убрать из списка</param>
        public async Task<bool> RemoveCityFromSelected(ICity city)
        {
            if (city == null) return false;
            bool ret = true;
            //Удаляем город из списка
            if (selectedCitys.Remove(city))
                //Если удаление произошло сохраняем итоговый список
                if ((!(await Sud.SaveUserCitys(selectedCitys.ToList<ICity>()))) && (selectedCitys.IndexOf(city) < 0))
                {
                    //Если при сохранении произошли ошибки и город в списке отсутствует, 
                    //то добавляем его обратно
                    selectedCitys.Add(city);
                    ret = false;
                }
            return ret;
        }

        /// <summary>
        /// Установка текущего расположения пользователя
        /// </summary>
        /// <param name="city">Город выбранный пользователем как его текущее расположение</param>
        public async Task<bool> SetUserCity(ICity city)
        {
            bool ret = true;
            ICity tCity = userCity;
            userCity = city;
            //Сохранить текущий город пользователя
            if (await Sud.SaveUserCurrentCity(userCity))
            {
                //попробовать удалить новый город пользователя из избранных
                if (selectedCitys.Remove(userCity))
                {
                    //Если удаление произошло, то пробежаться по списку избранных для удаления всех копий
                    while (selectedCitys.Remove(userCity)) ;
                    //сохранить почищенный список избранных
                    await Sud.SaveUserCitys(selectedCitys);
                }
            }
            else
            {
                userCity = tCity;
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Получить текущий город пользователя
        /// </summary>
        /// <returns>Текущий город пользователя</returns>
        public async Task<ICity> GetUserCity()
        {
            if (!FlagInitUserCity)
            {
                userCity = await Sud.LoadUserCurrentCity();
                FlagInitUserCity = true;
            }
            return userCity;
        }

        /// <summary>
        /// Получить список избранных городов пользователя
        /// </summary>
        /// <returns>Список избрайнных городов</returns>
        public async Task<List<ICity>> GetSelectedCitys()
        {
            if (!FlagInitSelectedCitys)
            {
                selectedCitys = await Sud.LoadUserCitys();
                FlagInitSelectedCitys = true;

            }
            return selectedCitys;
        }

        public UserCitys(ISaveUserData sud, ICitys citys, IWeatherForecast _wf)
        {
            Sud = sud;
            Citys = citys;
            wf = _wf;

            Init();
        }

        /// <summary>
        /// Возвращает список доступных городов для выбора, за искючением текущего и избранных на данный момент
        /// </summary>
        /// <returns>список доступных городов для выбора, за искючением текущего и избранных на данный момент</returns>
        public async Task<List<ICity>> getListUserForAddNewCityInFavorites()
        {
            List<ICity> lc = await Citys.getCitys();
            //Удалить из списка городов для выбора теущий город
            ICity UserCity = await GetUserCity();
            if (UserCity != null)
                while (lc.Remove(UserCity)) ;
            //Удалить из списка для выбора уже выбранные города
            List<ICity> SelectedCitys = await GetSelectedCitys();
            if ((SelectedCitys != null) && (SelectedCitys.Count > 0))
                foreach (ICity c in SelectedCitys)
                    while (lc.Remove(c)) ;
            return lc;
        }

        /// <summary>
        /// Возвращает список доступных городов для выбора текущего города
        /// </summary>
        /// <returns>список доступных городов для выбора</returns>
        public async Task<List<ICity>> getListUserForSelectUserCity()
        {
            return await Citys.getCitys();
        }

        /// <summary>
        /// Обновление данных о погоде
        /// </summary>
        async void UpdateForecastForUserCitys()
        {
            #region Постановка задач на и ожидание получения прогноза погоды
            //Список задач обновления погода
            List<Task<IForecastWeatherInCity>> pForEach = new List<Task<IForecastWeatherInCity>>();
            //в список задач
            ICity UserCity = await GetUserCity();
            if (UserCity != null)
                pForEach.Add(Task<IForecastWeatherInCity>.Factory.StartNew(()=> wf.GetWeatherForecastFromCity(UserCity)));
            else
                pForEach.Add(Task<IForecastWeatherInCity>.Factory.StartNew(() => wf.GetWeatherForecastFromCity(UserCity)));

            List<ICity> SelectedCitys = await GetSelectedCitys();
            if ((SelectedCitys != null) && (SelectedCitys.Count > 0))
                foreach (ICity c in SelectedCitys)
                    pForEach.Add(Task<IForecastWeatherInCity>.Factory.StartNew(() => wf.GetWeatherForecastFromCity(UserCity)));
            await Task.WhenAll(pForEach);
            #endregion

            List<IForecastWeatherInCity> lF = new List<IForecastWeatherInCity>();
            foreach (Task<IForecastWeatherInCity> tF in pForEach)
            {
                //Если задача закончилась без ошибок и результат не null, то добавить в список
                if ((!tF.IsFaulted) && (tF.Result != null))
                    lF.Add(tF.Result);
            }

            //Присвоить новый список прогнозов и уведомить подписантов
            lForecastForUserCitys = lF;

            if (UpdateForecast != null)
                UpdateForecast(this, new WeatherForecastEventArgs(lForecastForUserCitys));

        }

        /// <summary>
        /// Инициализация объекта
        /// </summary>
        async void Init()
        {
            ICity UserCity = await GetUserCity();
            //Если пользователем не указанно свое положение
            if (UserCity == null)
            {
                //Если есть возможность получить геопозицию (геолокация не откючена и доступна на устройстве)
                if ((gLoсator.LocationStatus != PositionStatus.Disabled) && (gLoсator.LocationStatus != PositionStatus.NotAvailable))
                {
                    CurrentPosition = CurrentCity;
                    gLoсator.PositionChanged += delegate (Geolocator sender, PositionChangedEventArgs e)
                    {
                        CurrentCity.lat = e.Position.Coordinate.Point.Position.Latitude;
                        CurrentCity.lon = e.Position.Coordinate.Point.Position.Longitude;
                        UpdateForecastForUserCitys();
                    };

                }
            }

            UpdateForecastForUserCitys();
            tpt = ThreadPoolTimer.CreatePeriodicTimer(delegate { UpdateForecastForUserCitys(); }, TimeSpan.FromMinutes(period));
        }
    }
}
