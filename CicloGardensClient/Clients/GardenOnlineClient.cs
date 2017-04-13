﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CicloGardensClient.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Debug = System.Diagnostics.Debug;

namespace CicloGardensClient.Clients
{
    public class GardenOnlineClient
    {
        private MobileServiceClient _client;
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
                _client = new MobileServiceClient("http://ciclogardens.azurewebsites.net");
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