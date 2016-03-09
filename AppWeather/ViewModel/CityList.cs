using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppWeather.ViewModel;
using System.ComponentModel;

namespace AppWeather.ViewModel
{
    public class VMCityList : INotifyPropertyChanged
    {
        private ObservableCollection<VMWeatherInCity> lCitys;
        /// <summary>
        /// Список городов
        /// </summary>
        public ObservableCollection<VMWeatherInCity> LCitys
        {
            get { return lCitys; }
            set
            {
                lCitys = value;
                OnPropertyChanged("LCitys");
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
