using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AppWeather.ViewModel
{
    /// <summary>
    /// Погода в городе
    /// </summary>
    public class VMWeatherInCity: INotifyPropertyChanged
    {
        private VMWeather current;
        /// <summary>
        /// Текущая погода
        /// </summary>
        public VMWeather Current
        {
            get { return current; }
            set
            {
                current = value;
                OnPropertyChanged("Current");
            }
        }

        private VMWeather morning;
        /// <summary>
        /// Погода утром
        /// </summary>
        public VMWeather Morning
        {
            get{ return morning; }
            set
            {
                morning = value;
                OnPropertyChanged("Morning");
            }
        }

        private VMWeather daytime;
        /// <summary>
        /// Погода днем
        /// </summary>
        public VMWeather Daytime
        {
            get { return daytime; }
            set
            {
                daytime = value;
                OnPropertyChanged("Daytime");
            }
        }

        private VMWeather afternoon;
        /// <summary>
        /// Погода вечером
        /// </summary>
        public VMWeather Afternoon
        {
            get { return afternoon; }
            set
            {
                afternoon = value;
                OnPropertyChanged("Afternoon");
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
