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
        public void DeleteGarden()
        {
            Task.Run(async () =>
            {
                await _client.Initializer;

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
    }
}
