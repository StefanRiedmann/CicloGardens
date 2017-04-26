using System;
using System.Linq;
using System.Threading.Tasks;
using CicloGardensClient.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CicloGardensClient.IntegrationTest
{
    [TestClass]
    public class GardenBlobClientTest
    {
        public GardenBlobClient Client { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Client = new GardenBlobClient();
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            Task.Run(async () =>
            {
                await Client.DeleteAllTestContainers();
                var containers = await Client.GetAllBlobContainers();
                Assert.IsFalse(containers.Any(c => c.Name.StartsWith("test")));
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TestDirect()
        {
            Task.Run(async () =>
            {
                var result = await Client.TestDirect();
                Assert.IsTrue(result);
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TestSass()
        {
            Task.Run(async () =>
            {
                var contRef = await Client.GetContainerReference("testsass");
                var policy = new SharedAccessBlobPolicy
                {
                    SharedAccessStartTime = DateTime.UtcNow,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(1),
                    Permissions = SharedAccessBlobPermissions.Create |
                                  SharedAccessBlobPermissions.Read |
                                  SharedAccessBlobPermissions.Write |
                                  SharedAccessBlobPermissions.List
                };
                var sas = contRef.GetSharedAccessSignature(policy);
                var uri = new Uri($"{contRef.Uri}{sas}");
                var sasContainer = new CloudBlobContainer(uri);

                var plantsDir = sasContainer.GetDirectoryReference("Plants");
                var blockBlobRef = plantsDir.GetBlockBlobReference("flower1");
                await blockBlobRef.UploadTextAsync("Beautiful");

            }).GetAwaiter().GetResult();
        }
    }
}