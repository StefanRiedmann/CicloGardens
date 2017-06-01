using System;
using CicloGardensClient.Clients;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using UIKit;
using Task = System.Threading.Tasks.Task;

namespace CicloGardens.Native.iOS
{
	public partial class ViewController : UIViewController
	{
		int count = 1;
	    private GardenOnlineClient _client;

	    public ViewController (IntPtr handle) : base (handle)
		{
		    _client = new GardenOnlineClient();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			Button.AccessibilityIdentifier = "myButton";
			//Button.TouchUpInside += delegate {
			//	var title = string.Format ("{0} clicks!", count++);
			//	Button.SetTitle (title, UIControlState.Normal);
			//};
            var profile = Profile.CurrentProfile;
            if (profile == null)
            {
                var loginView = new LoginButton(Button.Frame)
                {
                    LoginBehavior = LoginBehavior.Browser
                };

                loginView.Completed += async (sender, e) => { await LoginView_Completed(sender, e); };

                Button.Hidden = true;
                Add(loginView);
            }
        }

	    private async Task LoginView_Completed(object sender, LoginButtonCompletedEventArgs loginButtonCompletedEventArgs)
        {
            var tokenString = AccessToken.CurrentAccessToken?.TokenString;
            var accessToken = new JObject(); 
            accessToken["access_token"] = tokenString;
            await _client.MobileServiceClient.LoginAsync(MobileServiceAuthenticationProvider.Facebook, accessToken);
        }

	    public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

