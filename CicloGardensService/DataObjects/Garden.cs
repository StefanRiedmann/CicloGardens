using Microsoft.Azure.Mobile.Server;

namespace CicloGardensService.DataObjects
{
    public class Garden : EntityData
    {
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}