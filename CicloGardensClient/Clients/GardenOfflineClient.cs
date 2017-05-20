using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CicloGardensClient.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Debug= System.Diagnostics.Debug;

namespace CicloGardensClient.Clients
{
    public class GardenOfflineClient: IGardenClient
    {
        private MobileServiceClient _client;
        private IMobileServiceSyncTable<Garden> _syncTable;
        private IMobileServiceTable<Garden> _table;
        private MobileServiceSQLiteStore _store;

        public MobileServiceClient MobileServiceClient => _client;

        public Task<bool> Initializer;

        public GardenOfflineClient()
        {
            Initializer = InitClientAndSync();
        }

        private async Task<bool> InitClientAndSync()
        {
            try
            {
                _client = new MobileServiceClient(Constants.Url);
                
                _table = _client.GetTable<Garden>();
                _store = new MobileServiceSQLiteStore(@"localstore.db");
                _store.DefineTable<Garden>();
                
                await _client.SyncContext.InitializeAsync(_store, StoreTrackingOptions.NotifyLocalAndServerOperations);

                _syncTable = _client.GetSyncTable<Garden>();

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
                await _syncTable.PullAsync(
                    "allGardenItems",
                    _syncTable.CreateQuery());
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
                var items = await _syncTable.ToEnumerableAsync();

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
                    await _syncTable.InsertAsync(garden);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"SetGarden error: {e.Message}");
                }
            }
            else
            {
                await _syncTable.UpdateAsync(garden);
            }
        }

        public async Task DeleteAsync(Garden garden)
        {
            await _syncTable.DeleteAsync(garden);
        }
    }
}