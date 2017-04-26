using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CicloGardensClient.Clients
{
    public class GardenBlobClient
    {
        public async Task DeleteAllTestContainers()
        {
            var containers = await GetAllBlobContainers();

            foreach (var cloudBlobContainer in containers)
            {
                if (cloudBlobContainer.Name.StartsWith("test"))
                    await cloudBlobContainer.DeleteAsync();
            }
        }

        public async Task<List<CloudBlobContainer>> GetAllBlobContainers()
        {
            if (!GetClient(out var client))
                return null;
            var result = await client.ListContainersSegmentedAsync(null);
            return result.Results.ToList();
        }


        public async Task<bool> TestDirect()
        {
            var container = await GetContainerReference("testcontainer");
            if (container == null)
                return false;

            var plantsDir = container.GetDirectoryReference("Plants");
            var blockBlobRef = plantsDir.GetBlockBlobReference("flower1");
            await blockBlobRef.UploadTextAsync("Beautiful");

            return true;
        }

        #region Helpers

        private const string Conn = "DefaultEndpointsProtocol=https;AccountName=ciclogardensstorage;AccountKey=VV+F63jc7uMXN9wenUDaK/oCXX+PKL15EZ7adOb+lv2zzaMEgERuGHNaSZD4xJlU6XyvK4nrnb1V60H0EwjFyw==;EndpointSuffix=core.windows.net";

        private static bool GetClient(out CloudBlobClient client)
        {
            if (!CloudStorageAccount.TryParse(Conn, out var cloudAccount))
            {
                client = null;
                return false;
            }
            client = cloudAccount.CreateCloudBlobClient();
            return true;
        }

        public async Task<CloudBlobContainer> GetContainerReference(string name)
        {
            if (!GetClient(out var client))
                return null;

            var container = client.GetContainerReference(name);
            try
            {
                if (!await container.CreateIfNotExistsAsync())
                    return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"GetContainerReference: {e.Message}");
                return null;
            }
            return container;
        }

        #endregion
    }
}