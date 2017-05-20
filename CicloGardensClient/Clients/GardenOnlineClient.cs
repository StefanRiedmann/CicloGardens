using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CicloGardensClient.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage.Blob;
using Debug = System.Diagnostics.Debug;

namespace CicloGardensClient.Clients
{
    public class GardenOnlineClient : IGardenClient
    {
        private MobileServiceClient _client;

        public MobileServiceClient MobileServiceClient => _client;

        private IMobileServiceTable<Garden> _table;

        public GardenOnlineClient()
        {
            InitClient();
        }

        private void InitClient()
        {
            try
            {
                _client = new MobileServiceClient(Constants.Url);
                _table = _client.GetTable<Garden>();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error initialising garden client: {e.Message}");
                throw;
            }
        }

        public async Task<ObservableCollection<Garden>> GetGardensAsync()
        {
            try
            {
                var items = await _table.ToEnumerableAsync();

                return new ObservableCollection<Garden>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine($"Invalid operation: {msioe.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception: {e.Message}");
            }
            return null;
        }

        public async Task<Garden> GetGardenAsync(string name)
        {
            try
            {
                var items = await _table.ToEnumerableAsync();
                var garden = items.FirstOrDefault(g => g.Name.Equals(name));
                return garden;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine($"Invalid operation: {msioe.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception: {e.Message}");
            }
            return null;
        }

        public async Task SetGardenAsync(Garden garden)
        {
            if (garden.Id == null)
            {
                try
                {
                    await _table.InsertAsync(garden);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"SetGarden error: {e.Message}");
                }
            }
            else
            {
                await _table.UpdateAsync(garden);
            }
        }

        public async Task DeleteAsync(Garden garden)
        {
            await _table.DeleteAsync(garden);
        }

        public async Task<CloudBlobContainer> GetGardenBlobContainer(string gardenId)
        {
            var blobUrl = await _client.InvokeApiAsync<string>(
                $"/api/Garden/GetToken/{gardenId}",
                HttpMethod.Get,
                null,
                CancellationToken.None);
            var blobClient = new CloudBlobContainer(new Uri(blobUrl));
            return blobClient;
        }
    }
}