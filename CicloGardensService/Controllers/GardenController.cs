using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using System.Web.Http.Results;
using Microsoft.Azure.Mobile.Server;
using CicloGardensService.DataObjects;
using CicloGardensService.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CicloGardensService.Controllers
{
    [Authorize]
    public class GardenController : TableController<Garden>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new CicloGardensContext();
            DomainManager = new EntityDomainManager<Garden>(context, Request);
        }

        // GET tables/Garden
        public IQueryable<Garden> GetAllGardens()
        {
            return Query();
        }

        // GET tables/Garden/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Garden> GetGarden(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Garden/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Garden> PatchGarden(string id, Delta<Garden> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Garden
        public async Task<IHttpActionResult> PostGarden(Garden item)
        {
            Garden current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Garden/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteGarden(string id)
        {
            return DeleteAsync(id);
        }

        [HttpGet, Route("api/Garden/UserInfo")]
        public string UserInfo()
        {
            return $"{User.Identity.Name} {User.Identity.AuthenticationType}";
        }
        
        [HttpGet, Route("api/Garden/GetToken/{id}")]
        public async Task<IHttpActionResult> GetToken(string id)
        {
            try
            {
                var garden = DomainManager.Lookup(id).Queryable.FirstOrDefault();
                if (garden == null)
                {
                    return
                        new ResponseMessageResult(
                            Request.CreateErrorResponse(HttpStatusCode.NotFound, new HttpError($"Garden {id} doesn't exist")));
                }
                var containerName = $"garden-{garden.Name.ToLower()}";
                var container = await GetContainerReference(containerName);
                var policy = new SharedAccessBlobPolicy
                {
                    SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                    Permissions = SharedAccessBlobPermissions.Create |
                                  SharedAccessBlobPermissions.Read |
                                  SharedAccessBlobPermissions.Write |
                                  SharedAccessBlobPermissions.List
                };
                var sas = container.GetSharedAccessSignature(policy);
                var url = $"{container.Uri}{sas}";
                Trace.WriteLine($"Created blob url for {id}: {url}");
                return new ResponseMessageResult(Request.CreateResponse(HttpStatusCode.OK, url));
            }
            catch (Exception e)
            {
                Trace.TraceError($"GetToken: {e.Message}");
                return
                    new ResponseMessageResult(
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                            new HttpError($"Error getting blob storage token for garden {id}")));

            }
        }

        private async Task<CloudBlobContainer> GetContainerReference(string name)
        {
            try
            {
                var client = GetClient();
                if (client == null)
                    throw new ApplicationException($"CloudBlobClient for {name} could not be created");
                var container = client.GetContainerReference(name);
                await container.CreateIfNotExistsAsync();
                return container;
            }
            catch (Exception e)
            {
                Trace.TraceError($"GetContainerReference: {e.Message}");
                throw;
            }
        }

        private static CloudBlobClient GetClient()
        {
            var conn = ConfigurationManager.ConnectionStrings["MS_AzureStorageAccountConnectionString"].ConnectionString;
            if (!CloudStorageAccount.TryParse(conn, out var cloudAccount))
            {
                return null;
            }
            return cloudAccount.CreateCloudBlobClient();
        }
    }
}