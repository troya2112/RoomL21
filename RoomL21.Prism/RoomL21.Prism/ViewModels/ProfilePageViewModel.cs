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
    public class ProfilePageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private string _document;
        private string _firstName;
        private string _lastName;
        private string _address;
        private string _phoneNumber;
        private OrganizerResponse _organizer;
        private InvitedResponse _invited;
        private DelegateCommand _saveCommand;
        private DelegateCommand _changePasswordCommand;

        public ProfilePageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Modify Profile";
            IsEnabled = true;
            if (Settings.UserType=="Organizer")
            {
                Organizer = JsonConvert.DeserializeObject<OrganizerResponse>(Settings.Organizer);
                Address = Organizer.Address;
                Document = Organizer.Document;
                FirstName = Organizer.FirstName;
                LastName = Organizer.LastName;
                PhoneNumber = Organizer.PhoneNumber;
            }
            else
            {
                Invited = JsonConvert.DeserializeObject<InvitedResponse>(Settings.Invited);
                Address = Invited.Address;
                Document = Invited.Document;
                FirstName = Invited.FirstName;
                LastName = Invited.LastName;
                PhoneNumber = Invited.PhoneNumber;
            }
            Title = "Modify Profile";
        }

        public DelegateCommand ChangePasswordCommand => _changePasswordCommand ?? (_changePasswordCommand = new DelegateCommand(ChangePassword));

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save));

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
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }
        public OrganizerResponse Organizer
        {
            get => _organizer;
            set => SetProperty(ref _organizer, value);
        }

        public InvitedResponse Invited
        {
            get => _invited;
            set => SetProperty(ref _invited, value);
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

        public IApiService ApiService => _apiService;

        private async void Save()
        {
            var isValid = await ValidateData();
            if (!isValid)
            {
                return;
            }

            var userRequest = AsingUserRequest();

            IsRunning = true;
            IsEnabled = false;

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var url = App.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.PutAsync(
                url,
                "/api",
                "/Account/PutUser",
                userRequest,
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
                return;
            }
            await App.Current.MainPage.DisplayAlert(
                "Ok",
                "User updated succesfully.",
                "Accept");

            if (Settings.UserType == "Organizer")
            {
                var response2 = await _apiService.GetOrganizerByEmailAsync(url, "api", "/Organizers/GetOrganizerByEmail", "bearer", token.Token, Organizer.Email);
                if (!response2.IsSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Please refresh the app", "Accept");
                    return;
                }
                var organizer = response2.Result;
                Settings.Organizer = JsonConvert.SerializeObject(organizer);
            }
            else
            {
                var response2 = await _apiService.GetInvitedByEmailAsync(url, "api", "/Inviteds/GetInvitedByEmail", "bearer", token.Token, Invited.Email);
                if (!response2.IsSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Please refresh the app", "Accept");
                    return;
                }
                var invited = response2.Result;
                Settings.Invited = JsonConvert.SerializeObject(invited);
            }
        }

        private UserRequest AsingUserRequest()
        {
            if (Settings.UserType == "Organizer")
            {
                return  new UserRequest
                {
                    Address = Address,
                    Document = Document,
                    Email = Organizer.Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Password = "123456", // It doesn't matter what is sent here. It is only for the model to be valid
                    Phone = PhoneNumber
                };
            }
            else
            {
                return new UserRequest
                {
                    Address = Address,
                    Document = Document,
                    Email = Invited.Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Password = "123456", // It doesn't matter what is sent here. It is only for the model to be valid
                    Phone = PhoneNumber
                };
            }
        }

        private async Task<bool> ValidateData()
        {

            if (string.IsNullOrEmpty(Organizer.Document))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a document.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(Organizer.FirstName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a FirstName.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(Organizer.LastName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a LastName.", "Accept");
                return false;
            }

            if (string.IsNullOrEmpty(Organizer.Address))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter an Address.", "Accept");
                return false;
            }           

            return true;
        }

        private async void ChangePassword()
        {
            await _navigationService.NavigateAsync("ChangePasswordPage");
        }
    }
}
