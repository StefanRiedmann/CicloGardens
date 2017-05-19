using Prism.Unity;
using CicloGardens.Views;
using CicloGardensClient.Clients;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace CicloGardens
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync("NavigationPage/MainPage?title=Hello%20from%20Xamarin.Forms");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();

            Container.RegisterType(typeof(IGardenClient), typeof(GardenOnlineClient),
                new ContainerControlledLifetimeManager());
        }
    }
}
