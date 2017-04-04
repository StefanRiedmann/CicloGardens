using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CicloGardensClient.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Debug= System.Diagnostics.Debug;

namespace CicloGardensClient.Clients
{
    public class GardenClient
    {
        private MobileServiceClient _client;
        private IMobileServiceSyncTable<Garden> _table;
        private MobileServiceSQLiteStore _store;

        public Task<bool> Initializer;

        public GardenClient()
        {
            Initializer = InitClientAndSync();
        }

        private async Task<bool> InitClientAndSync()
        {
            try
            {
                _client = new MobileServiceClient("http://ciclogardens.azurewebsites.net");
                _table = _client.GetSyncTable<Garden>();
                _store = new MobileServiceSQLiteStore(@"localstore.db");
                _store.DefineTable<Garden>();
                await _client.SyncContext.InitializeAsync(_store, StoreTrackingOptions.NotifyLocalAndServerOperations);
                await Sync();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error initialising garden client: {e.Message}");
                return false;
            }
        }

        public async Task<bool> Sync()
        {
            try
            {
                await _client.SyncContext.PushAsync();
                await _table.PullAsync(
                    "allGardenItems",
                    _table.CreateQuery());
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error syncing gardens: {e.Message}");
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
    }
}