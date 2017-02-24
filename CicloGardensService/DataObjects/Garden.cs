using Microsoft.Azure.Mobile.Server;

namespace CicloGardensService.DataObjects
{
    public class Garden : EntityData
    {
        public string Name { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}