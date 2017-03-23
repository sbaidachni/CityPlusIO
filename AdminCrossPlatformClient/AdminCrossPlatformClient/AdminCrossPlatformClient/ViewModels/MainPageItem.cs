using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminCrossPlatformClient.ViewModels
{
    public class MainPageItem: INotifyPropertyChanged
    {
        private int _Id;
        private string _Name;
        private string _Address;
        private double _Lon;
        private double _Lat;
        private bool _Medicine;
        private bool _Food;
        private bool _Clothes;
        private bool _Shelter;

        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Id"));
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Address"));
            }
        }

        public double Lon
        {
            get { return _Lon; }
            set
            {
                _Lon = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Lon"));
            }
        }

        public double Lat
        {
            get { return _Lat; }
            set
            {
                _Lat = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Lat"));
            }
        }

        public bool Shelter
        {
            get { return _Shelter; }
            set
            {
                _Shelter = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Shelter"));
            }
        }

        public bool Medicine
        {
            get { return _Medicine; }
            set
            {
                _Medicine = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Medicine"));
            }
        }

        public bool Food
        {
            get { return _Food; }
            set
            {
                _Food = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Food"));
            }
        }

        public bool Clothes
        {
            get { return _Clothes; }
            set
            {
                _Clothes = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Clothes"));
            }
        }

        public async Task<bool> CheckAddressAsync()
        {
            return true;
        }

        public async Task NotifyUsersAsync()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
