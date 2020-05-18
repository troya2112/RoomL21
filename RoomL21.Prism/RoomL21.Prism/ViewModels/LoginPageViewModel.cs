using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using RoomL21.Common.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RoomL21.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private UserTypeResponse _userType;
        private ObservableCollection<UserTypeResponse> _userTypes;
        private DelegateCommand _loginCommand;
        private DelegateCommand _registerCommand;
        private DelegateCommand _forgotPasswordCommand;

        public LoginPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Room L21 Event - Login";
            IsEnabled = true;
            IsRemember = true;
            IsRunning = false;
        }

        public DelegateCommand ForgotPasswordCommand => _forgotPasswordCommand ?? (_forgotPasswordCommand = new DelegateCommand(ForgotPassword));

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(Register));

        public bool IsRemember { get; set; }

        public string Email { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }
        public UserTypeResponse UserType
        {
            get => _userType;
            set => SetProperty(ref _userType, value);
        }
        public ObservableCollection<UserTypeResponse> UserTypes
        {
            get => _userTypes;
            set => SetProperty(ref _userTypes, value);
        }

        private async void Login()
        {
            if (UserType is null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must select a user type", "Accept");
                return;
            }
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter an email.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a password.", "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var connection = await _apiService.CheckConnection(url);
            if (!connection)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Error", "Check the internet connection.", "Accept");
                return;
            }

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Email or password incorrect.", "Accept");
                Password = string.Empty;
                return;
            }
            var token = response.Result;
            if (UserType.Value == "Organizer")
            {
                var response2 = await _apiService.GetOrganizerByEmailAsync(url, "api", "/Organizers/GetOrganizerByEmail", "bearer", token.Token, Email);
                if (!response2.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await App.Current.MainPage.DisplayAlert("Error", "You did select an incorrect user type", "Accept");
                    return;
                }

                var organizer = response2.Result;
                Settings.Organizer = JsonConvert.SerializeObject(organizer);
                Settings.Token = JsonConvert.SerializeObject(token);
                Settings.IsRemembered = IsRemember;
                Settings.UserType = UserType.Value;

                IsRunning = false;
                IsEnabled = true;
                await NavigationService.NavigateAsync("/L21MasterDetailPage/NavigationPage/EventsPage");
                Password = string.Empty;
            }
            else
            {
                var response2 = await _apiService.GetInvitedByEmailAsync(url, "api", "/Inviteds/GetInvitedByEmail", "bearer", token.Token, Email);
                if (!response2.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await App.Current.MainPage.DisplayAlert("Error", "You did select an incorrect user type", "Accept");
                    return;
                }

                var invited = response2.Result;
                Settings.Invited = JsonConvert.SerializeObject(invited);
                Settings.Token = JsonConvert.SerializeObject(token);
                Settings.IsRemembered = IsRemember;
                Settings.UserType = UserType.Value;

                IsRunning = false;
                IsEnabled = true;
                await NavigationService.NavigateAsync("/L21MasterDetailPage/NavigationPage/EventTabbedPage");
                Password = string.Empty;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            LoadUserTypesAsync();
        }

        private async void LoadUserTypesAsync()
        {
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();

            var connection = await _apiService.CheckConnection(url);
            if (!connection)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Check the internet connection.",
                    "Accept");
                return;
            }

            var response = await _apiService.GetUserTypesAsync<UserTypeResponse>(
                url,
                "/api",
                "/UserTypes",
                "/GetUserTypes");

            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Getting the user types list, please try later,",
                    "Accept");
                return;
            }

            var userTypes = (List<UserTypeResponse>)response.Result;
            UserTypes = new ObservableCollection<UserTypeResponse>(userTypes);

        }

        private async void Register()
        {
            await _navigationService.NavigateAsync("RegisterPage");
        }

        private async void ForgotPassword()
        {
            await _navigationService.NavigateAsync("RememberPasswordPage");
        }

    }
}
