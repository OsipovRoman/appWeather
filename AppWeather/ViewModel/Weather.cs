using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace AppWeather.ViewModel
{
    /// <summary>
    /// Данные о погоде
    /// </summary>
    public class VMWeather : INotifyPropertyChanged
    {
        private double temp;
        /// <summary>
        /// Температура
        /// </summary>
        public double Temp
        {
            get { return temp; }
            set
            {
                temp = value;
                OnPropertyChanged("Temp");
            }
        }

        private string weather;
        /// <summary>
        /// Описание погоды
        /// </summary>
        public string Weather
        {
            get { return weather; }
            set
            {
                weather = value;
                OnPropertyChanged("Weather");
            }
        }

        private double wind;
        /// <summary>
        /// Скорость ветра
        /// </summary>
        public double Wind
        {
            get { return wind; }
            set
            {
                wind = value;
                OnPropertyChanged("Wind");
            }
        }

        private double wind_deg;
        /// <summary>
        /// Направление ветра
        /// </summary>
        public double Wind_deg
        {
            get { return wind_deg; }
            set
            {
                wind_deg = value;
                OnPropertyChanged("Wind_deg");
            }
        }


        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
