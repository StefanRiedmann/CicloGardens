using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using CicloGardensService.DataObjects;
using CicloGardensService.Models;

namespace CicloGardensService.Controllers
{
    public class GardenController : TableController<Garden>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            CicloGardensContext context = new CicloGardensContext();
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
    }
}