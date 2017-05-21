using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CicloGardensClient.Clients;
using CicloGardensClient.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CicloGardensClient.IntegrationTest
{
    [TestClass]
    public class GardenOnlineClientTest
    {
        private GardenOnlineClient _client;

        [TestInitialize]
        public void Setup()
        {
            _client = new GardenOnlineClient();
        }

        [TestMethod]
        public void SetAndGetGardenTest()
        {
            Task.Run(async () =>
            {
                var watch = Stopwatch.StartNew();
                await _client.SetGardenAsync(new Garden { Name = "NewGarden1" });
                watch.Stop();
                Debug.WriteLine($"Time setting one new garden: ${watch.ElapsedMilliseconds}");

                var garden = _client.GetGardensAsync().Result.FirstOrDefault(g => g.Name == "NewGarden1");
                Debug.WriteLine($"Id: ${garden?.Id}");
                Assert.IsNotNull(garden);
                Assert.IsFalse(string.IsNullOrEmpty(garden?.Id));

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void DeleteGardenTest()
        {
            Task.Run(async () =>
            {
                await _client.SetGardenAsync(new Garden { Name = "NewGarden2" });

                var all = await _client.GetGardensAsync();
                Debug.WriteLine($"First count: {all.Count}");
                Assert.IsTrue(all.Count > 0);

                var watch = Stopwatch.StartNew();
                foreach (var g in all)
                {
                    await _client.DeleteAsync(g);
                }
                Debug.WriteLine($"Time for deleting ${all.Count} garden(s): ${watch.ElapsedMilliseconds}");

                all = await _client.GetGardensAsync();
                Debug.WriteLine($"Second count: {all.Count}");
                Assert.AreEqual(0, all.Count);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void GetGardenBlobTest()
        {
            Task.Run(async () =>
            {
                //Arrange
                var testGardenName = $"test-{Guid.NewGuid()}";
                await _client.SetGardenAsync(new Garden { Name = testGardenName });
                var garden = await _client.GetGardenAsync(testGardenName);
                //Act
                var container = await _client.GetGardenBlobContainer(garden.Id);
                var plantsDir = container.GetDirectoryReference("Plants");
                var blockBlobRef = plantsDir.GetBlockBlobReference("flower1");
                await blockBlobRef.UploadTextAsync("Beautiful");
                //Assert Directory
                var directories = container.ListBlobs().ToList();
                Assert.AreEqual(1, directories.Count);

                var plantsDirRead = directories[0] as CloudBlobDirectory;
                Assert.IsNotNull(plantsDirRead);
                Assert.AreEqual("Plants/", plantsDirRead.Prefix);

                //Assert Directory content
                var dirContent = plantsDirRead.ListBlobs().ToList();
                Assert.AreEqual(1, dirContent.Count);

                var plantRead = dirContent[0] as CloudBlockBlob;
                Assert.IsNotNull(plantRead);
                Assert.AreEqual("Plants/flower1", plantRead.Name);

                //Assert 'file' content
                var content = await plantRead.DownloadTextAsync();
                Assert.AreEqual("Beautiful", content);

            }).GetAwaiter().GetResult();
        }


        [TestMethod]
        public void GetUserInfo()
        {
            //Task.Run(async () => {
            //    _client.MobileServiceClient..
            //    var info = await _client.GetUserInfo();
            //    Debug.WriteLine($"Info: {info}");
            //    Assert.IsNotNull(info);
            //}).GetAwaiter().GetResult();
        }
    }
}
