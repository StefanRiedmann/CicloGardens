using System.Data;
using System.Xml;
using Microsoft.Azure.Mobile.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace CicloGardensService.DataObjects
{
    public class Garden : EntityData
    {
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}