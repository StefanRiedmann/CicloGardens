using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CicloGardensService.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CicloGardensService.IntegrationTest
{
    /// <summary>
    /// Manually in Postman:
    /// http://ciclogardens.azurewebsites.net/tables/Garden
    /// zumo-api-version 2.0.0
    /// Content-Type application/json
    /// Body - raw - JSON (for POST)
    /// </summary>
    [TestClass]
    public class GardenControllerTest
    {
        public const string BaseUrl = "http://ciclogardens.azurewebsites.net";

        [TestMethod]
        public void GetAllGardens()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var gardens = new List<Garden>(new[]
            {
                new Garden
                {
                    Name = "FirstGarden",
                    Latitude = -32.8772299,
                    Longitude = -68.851897,
                }
            });
            var jsonObjects = JsonConvert.SerializeObject(gardens[0]);
             var postResponse =
                client.PostAsync($"{BaseUrl}/tables/Garden?zumo-api-version=2.0.0", new StringContent(jsonObjects, Encoding.UTF32, "application/json"))
                    .Result;

            //var getResponse = client.GetAsync($"{BaseUrl}/tables/Garden?zumo-api-version=2.0.0").Result;
            //var result = getResponse.Content.ReadAsStringAsync().Result;
        }

        [TestMethod]
        public void GetGarden()
        {
        }

        [TestMethod]
        public void PatchGarden()
        {
        }

        [TestMethod]
        public void PostGarden()
        {
        }

        [TestMethod]
        public void DeleteGarden()
        {
        }
    }
}
