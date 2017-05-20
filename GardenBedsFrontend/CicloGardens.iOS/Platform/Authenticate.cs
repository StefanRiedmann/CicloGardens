using System;
using System.Threading.Tasks;
using CicloGardens.Platform;
using CicloGardensClient.Clients;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;

namespace CicloGardens.iOS.Platform
{
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
                var user = await _client.MobileServiceClient.LoginAsync(
                    UIApplication.SharedApplication.KeyWindow.RootViewController,
                    MobileServiceAuthenticationProvider.Google);
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