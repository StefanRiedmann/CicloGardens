using System;
using System.Diagnostics;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using CicloGardens.Platform;
using CicloGardensClient.Clients;

namespace CicloGardens.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private readonly IGardenClient _gardenClient;
        private readonly IAuthenticate _authenticator;

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _user;
        public string User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private string _server;
        public string Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); }
        }

        public MainPageViewModel(IGardenClient gardenClient, IAuthenticate authenticator)
        {
            _gardenClient = gardenClient;
            _authenticator = authenticator;
            Login = new DelegateCommand(async () =>
            {
                Title = "Authenticating...";
                var success = await _authenticator.AuthenticateAsync();
                if (success)
                {
                    Title = "Facebook Auth passed";
                    User = _gardenClient.MobileServiceClient.CurrentUser.UserId;
                    Server = await _gardenClient.GetUserInfo();
                }
                else
                {
                    Title = "Facebook Auth failed";
                    User = null;
                    Server = null;
                }
            });
            Logout = new DelegateCommand(async () =>
            {
                Title = "Logging out...";
                var success = await _authenticator.Logout();
                if (success)
                {
                    Title = "Facebook logged out";
                    User = null;
                    Server = null;
                }
                else
                {
                    Title = "Facebook logout failed";
                }
            });
            App.PostSuccessFacebookAction = token =>
            {
                Title = $"From native: {token}";
            };

            MessAround();
        }

        public async void MessAround()
        {
            try
            {
                User = _gardenClient.MobileServiceClient?.CurrentUser?.UserId;
                Server = await _gardenClient.GetUserInfo(); ;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MessAround: {e.Message}");
                User = $"MessAround: {e.Message}";
            }
        }

        public DelegateCommand Login { get; set; }
        public DelegateCommand Logout { get; set; }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("title"))
                Title = (string)parameters["title"] + " and Prism";
        }
    }
}
