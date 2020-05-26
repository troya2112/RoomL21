using Newtonsoft.Json;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using RoomL21.Common.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomL21.Prism.ViewModels
{
    public class AddInvitedsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private string _document;
        private string _firstName;
        private string _lastName;
        private string _address;
        private string _email;
        private string _phone;
        private int _invitedsNumber;
        private EventResponse _event;
        private DelegateCommand _registerCommand;
        private DelegateCommand _backToEventsCommand;

        public AddInvitedsPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            IsRunning = false;
            IsEnabled = true;
            Title = "Add guests";
        }

        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(Register));
        public DelegateCommand BackToEventsCommand => _backToEventsCommand ?? (_backToEventsCommand = new DelegateCommand(BackToEvents));

        public string Document
        {
            get => _document;
            set => SetProperty(ref _document, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
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

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            _event = JsonConvert.DeserializeObject<EventResponse>(Settings.Event);
            await ValidateInvitedsNumber();
        }
        private async void BackToEvents()
        {
            CleanData();
            await NavigationService.NavigateAsync("/RoomL21MasterDetailPage/NavigationPage/EventsPage");
        }


        private async void Register()
        {
            var isValid = await ValidateData();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var request = new UserRequest
            {
                Address = Address,
                Document = Document,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Password = Document,
                Phone = Phone,
                Role = "Invited",
                EventId = _event.Id
            };

            var url = App.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.RegisterUserAsync(
                url,
                "api",
                "/Account/RegisterUser",
                request);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }

            await App.Current.MainPage.DisplayAlert(
                "Ok",
                $"{response.Message} Please enter another guest",
                "Accept");
            CleanData();
            await ValidateInvitedsNumber();
        }

        private void CleanData()
        {
            Document = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Address = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
        }
        private async Task<bool> ValidateInvitedsNumber()
        {
            IsRunning = true;
            IsEnabled = false;
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var url = App.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.GetListAsync<InvitedResponse>(
            url,
            "/api",
            "/Inviteds/GetInviteds",
            "bearer",
            token.Token);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return false ;
            }
            var eventVar = JsonConvert.DeserializeObject<EventResponse>(Settings.Event);
            var inviteds = (List<InvitedResponse>)response.Result;
            _invitedsNumber = inviteds.Where(i => i.EventId == eventVar.Id).Count();
            Title = $"Add guests number {_invitedsNumber + 1}";
            return true;
        }
        private async Task<bool> ValidateData()
        {
            var _eventVar = JsonConvert.DeserializeObject<EventResponse>(Settings.Event);
            if (_invitedsNumber>=_eventVar.InvitedsNumber)
            {
                await App.Current.MainPage.DisplayAlert("Error", "You cannot invite more people than the event has", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(Document))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a document.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a firstname.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a lastname.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(Address))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter an address.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(Email) || !RegexHelper.IsValidEmail(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a valid email.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(Phone))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a phone.", "Accept");
                return false;
            }
            return true;
        }
    }
}
