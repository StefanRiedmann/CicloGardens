using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using CicloGardens.Droid.Platform;
using CicloGardens.Platform;
using Prism.Unity;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure.MobileServices;

namespace CicloGardens.Droid
{
    [Activity(Label = "CicloGardens", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Context { get; private set; }

        public MainActivity()
        {
            Context = this;
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.tabs;
            ToolbarResource = Resource.Layout.toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidInitializer()));
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.FirstUser)
            {
                var token = data.GetStringExtra("facebookToken");
                App.PostSuccessFacebookAction(token);
            }

        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType(typeof(IAuthenticate), typeof(Authenticate),
                new ContainerControlledLifetimeManager());
        }
    }
}

