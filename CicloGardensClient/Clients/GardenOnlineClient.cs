using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CicloGardensClient.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Debug = System.Diagnostics.Debug;

namespace CicloGardensClient.Clients
{
    public class GardenOnlineClient
    {
        //private const string Url = "http://ciclogardens.azurewebsites.net";
        private const string Url = "http://localhost:50271";
        private MobileServiceClient _client;
        private HttpClient _httpClient;

        private IMobileServiceTable<Garden> _table;

        public Task<bool> Initializer;

        public GardenOnlineClient()
        {
            Initializer = InitClientAndSync();
        }

        private async Task<bool> InitClientAndSync()
        {
            try
            {
                _client = new MobileServiceClient(Url);
                _httpClient = new HttpClient();
                _table = _client.GetTable<Garden>();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error initialising garden client: {e.Message}");
                return false;
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
            var request = WebRequest.CreateHttp($"{Url}/tables/Garden/GetToken/{gardenId}");
            request.ContentType = "application/json";
            request.Headers["zumo-api-version"] = "2.0.0";
            var response = await request.GetResponseAsync();
            var reader = new StreamReader(response.GetResponseStream());
            var str = reader.ReadToEnd();
            var blobUrl = JsonConvert.DeserializeObject<string>(str);
            //return null;

            //var url = $"{Url}/tables/Garden/GetToken/{gardenId}?zumo-api-version=2.0.0";
            ////http://ciclogardens.azurewebsites.net/tables/Garden/8dca10e8e6994a55994afb693f86d790?zumo-api-version=2.0.0
            //var result = await _httpClient.GetAsync(url);
            //var blobUrl = await result.Content.ReadAsStringAsync();
            var blobClient = new CloudBlobContainer(new Uri(blobUrl));
            return blobClient;
        }
    }
}