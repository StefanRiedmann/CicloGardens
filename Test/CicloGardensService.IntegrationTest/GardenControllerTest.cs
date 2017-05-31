using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;
using CicloGardensClient.Clients;
using CicloGardensService.DataObjects;
using CicloGardensService.Models;
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
        private readonly HttpClient _client;

        private List<Garden> _gardens ;

        #region Construct and Initialize

        public GardenControllerTest()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [TestInitialize]
        public void Init()
        {
            var ctx = new CicloGardensContext();
            ctx.Database.ExecuteSqlCommand("DELETE FROM dbo.Gardens");
            var count = ctx.Garden.Count();
            if (count > 0) throw new AssertFailedException("Table should be empty now");

            _gardens = new List<Garden>(new[]
            {
                new Garden
                {
                    Name = "FirstGarden",
                    Latitude = -32.8772299,
                    Longitude = -68.851897,
                },
                new Garden
                {
                    Name = "SecondGarden",
                    Latitude = -33.5772299,
                    Longitude = -68.96,
                }
            });
        }

        #endregion
        
        #region Tests

        [TestMethod]
        public void PostGarden()
        {
            //Arrange, Act
            PostDefaultGardens();
            //Assert
            Assert.IsNotNull(_gardens[0].Id);
            Assert.IsNotNull(_gardens[0].CreatedAt);
            Assert.IsNotNull(_gardens[1].Id);
            Assert.IsNotNull(_gardens[1].CreatedAt);
        }

        [TestMethod]
        public void GetAllGardens()
        {
            //Arrange
            PostDefaultGardens();
            //Act
            var result = GetAll().OrderBy(g => g.Name).ToList();
            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("FirstGarden", result[0].Name);
            Assert.AreEqual("SecondGarden", result[1].Name);
        }

        [TestMethod]
        public void GetGarden()
        {
            //Arrange
            PostDefaultGardens();
            //Act
            var uri = GetUriWithId(_gardens[0].Id);
            Debug.WriteLine($"uri: {uri}");
            var getResponse = _client.GetAsync(uri).Result;
            var result = JsonConvert.DeserializeObject<Garden>(getResponse.Content.ReadAsStringAsync().Result);
            //Assert
            Assert.AreEqual("FirstGarden", result.Name);
        }

        [TestMethod]
        public void PatchGarden()
        {
            //Arrange
            PostDefaultGardens();

            //Act
            var garden = _gardens[0];
            var deltaGarden = new Delta<Garden>();
            deltaGarden.TrySetPropertyValue("Name", "NewName");
            deltaGarden.TrySetPropertyValue("Longitude", 32d);
            deltaGarden.TrySetPropertyValue("Latitude", 27d);
            var json = JsonConvert.SerializeObject(deltaGarden, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var patchResponse =
                PatchAsync(GetUriWithId(garden.Id), new StringContent(json, Encoding.UTF8, "application/json")).Result;

            var getResponse = _client.GetAsync(GetUriWithId(garden.Id)).Result;
            var result = JsonConvert.DeserializeObject<Garden>(getResponse.Content.ReadAsStringAsync().Result);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, patchResponse.StatusCode);
            Assert.AreEqual(garden.Id, result.Id);
            Assert.AreEqual("NewName", result.Name);
            Assert.AreEqual(32d, result.Longitude);
            Assert.AreEqual(27d, result.Latitude);
            Assert.AreNotEqual(result.CreatedAt, result.UpdatedAt);
        }

        [TestMethod]
        public void DeleteGarden()
        {
            //Arrange
            PostDefaultGardens();
            //Act
            var deleteResponse = _client.DeleteAsync(GetUriWithId(_gardens[0].Id)).Result;
            var all = GetAll();
            //Assert
            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            Assert.AreEqual(1, all.Count);
        }

        #endregion
        
        #region Helpers

        private void PostDefaultGardens()
        {
            var createdGardens = new List<Garden>();
            _gardens.ForEach(garden =>
            {
                var jsonObjects = JsonConvert.SerializeObject(garden);
                var postResponse =
                    _client.PostAsync(GetDefaultUri(),
                        new StringContent(jsonObjects, Encoding.UTF8, "application/json")).Result;
                Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
                var createdGarden = JsonConvert.DeserializeObject<Garden>(postResponse.Content.ReadAsStringAsync().Result);
                createdGardens.Add(createdGarden);
            });
            _gardens = createdGardens;
        }

        private List<Garden> GetAll()
        {
            var getResponse = _client.GetAsync(GetDefaultUri()).Result;
            var result = JsonConvert.DeserializeObject<List<Garden>>(getResponse.Content.ReadAsStringAsync().Result);
            return result;
        }

        private string GetDefaultUri()
        {
            return $"{Constants.CurrentUrl}/tables/Garden?zumo-api-version=2.0.0";
        }

        private string GetUriWithId(string id)
        {
            return $"{Constants.CurrentUrl}/tables/Garden/{id}?zumo-api-version=2.0.0";
        }

        private async Task<HttpResponseMessage> PatchAsync(string uri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, uri)
            {
                Content = content
            };
            var response = await _client.SendAsync(request);
            return response;
        }

        #endregion
    }
}
