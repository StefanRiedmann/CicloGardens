using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CicloGardensService.Startup))]
namespace CicloGardensService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}