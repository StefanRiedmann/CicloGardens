using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CicloGardensClient.Clients;
using CicloGardensClient.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CicloGardensClient.IntegrationTest
{
    [TestClass]
    public class GardenOfflineClientTest
    {
        private GardenOfflineClient _client;

        [TestInitialize]
        public void Setup()
        {
            _client = new GardenOfflineClient();
        }

        [TestMethod]
        public void InstantiateGardenClient()
        {
            var initResult = _client.Initializer.Result;
            Assert.IsTrue(initResult);
        }

        [TestMethod]
        public void SetAndGetGarden()
        {
            Task.Run(async () =>
            {
                await _client.Initializer;

                var watch = Stopwatch.StartNew();
                await _client.Sync();
                await _client.SetGardenAsync(new Garden { Name = "NewGarden1" });
                watch.Stop();
                Debug.WriteLine($"Time for syncing one new garden: {watch.ElapsedMilliseconds}");

                var garden = _client.GetGardensAsync().Result.FirstOrDefault(g => g.Name == "NewGarden1");
                Debug.WriteLine($"Id: {garden?.Id}");
                Assert.IsNotNull(garden);
                Assert.IsFalse(string.IsNullOrEmpty(garden?.Id));

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void DeleteGarden()
        {
            Task.Run(async () =>
            {
                await _client.Initializer;

                await _client.SetGardenAsync(new Garden { Name = "NewGarden2" });
                await _client.Sync();

                var all = await _client.GetGardensAsync();
                Debug.WriteLine($"Deleting {all.Count} garden(s)...");

                var watch = Stopwatch.StartNew();
                foreach (var g in all)
                {
                    await _client.DeleteAsync(g);
                }
                await _client.Sync();
                Debug.WriteLine($"Time for deleting and syncing {all.Count} garden(s): {watch.ElapsedMilliseconds}ms");

                all = await _client.GetGardensAsync();
                Debug.WriteLine($"Second count: {all.Count}");
                Assert.AreEqual(0, all.Count);

            }).GetAwaiter().GetResult();
        }

        private void ResourceTest()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            foreach (var res in resources)
            {
                Debug.WriteLine($"Resource: {res}");
            }
        }
    }
}
