using CicloGardensClient.DataObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace CicloGardensClient.Clients
{
    public interface IGardenClient
    {
        MobileServiceClient MobileServiceClient { get; }
        Task<ObservableCollection<Garden>> GetGardensAsync();
        Task<Garden> GetGardenAsync(string name);
        Task SetGardenAsync(Garden garden);
        Task DeleteAsync(Garden garden);
        Task<string> GetUserInfo();
    }
}
