using System;
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
                }
                else
                {
                    Title = "Facebook Auth failed";
                    User = null;
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
                }
                else
                {
                    Title = "Facebook logout failed";
                }
            });
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
