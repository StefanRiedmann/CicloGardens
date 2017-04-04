using Prism.Mvvm;

namespace CicloGardensClient.DataObjects
{
    public class Garden : BindableBase
    {
        private string _id;
        private string _name;
        private double _latitude;
        private double _longitude;

        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        
        public double Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }
        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }
    }
}