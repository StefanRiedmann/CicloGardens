using System;
using System.Threading.Tasks;
using CicloGardens.Platform;
using CicloGardensClient.Clients;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Facebook.Login;
using System.Collections.Generic;

namespace CicloGardens.Droid.Platform
{
    //server flow: https://developer.xamarin.com/guides/xamarin-forms/cloud-services/authentication/azure/
    //native: https://developers.facebook.com/docs/facebook-login/android
    public class Authenticate : IAuthenticate
    {
        private readonly IGardenClient _client;

        public Authenticate(IGardenClient client)
        {
            _client = client;
        }
        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                var loginMgr = DeviceLoginManager.Instance;
                loginMgr.LogInWithReadPermissions(MainActivity.Context, new List<string>(new[] { "" }));

                var user = await _client.MobileServiceClient.LoginAsync(MainActivity.Context, MobileServiceAuthenticationProvider.Facebook);
                System.Diagnostics.Debug.WriteLine($"User logged in: {user.UserId}");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging in: {e.Message}");
                return false;
            }
            return true;
        }

        public async Task<bool> Logout()
        {
            try
            {
                await _client.MobileServiceClient.LogoutAsync();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Logout error: {e.Message}");
                return false;
            }
            return true;
        }
    }
}