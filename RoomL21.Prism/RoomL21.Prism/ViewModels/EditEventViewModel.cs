using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using RoomL21.Common.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RoomL21.Prism.ViewModels
{
    public class EditEventViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private EventResponse _event;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isEdit;
        public string _saveText;
        private ObservableCollection<EventTypeResponse> _eventTypes;
        private EventTypeResponse _eventType;
        private ObservableCollection<InvitedNumberResponse> _invitedNumbers;
        private InvitedNumberResponse _invitedNumber;
        private DelegateCommand _saveCommand;
        private OrganizerResponse _organizer;

        public EditEventViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Add or Edit a Event";
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));

        public ObservableCollection<EventTypeResponse> EventTypes
        {
            get => _eventTypes;
            set => SetProperty(ref _eventTypes, value);
        }

        public EventTypeResponse EventType
        {
            get => _eventType;
            set => SetProperty(ref _eventType, value);
        }
        public ObservableCollection<InvitedNumberResponse> InvitedNumbers
        {
            get => _invitedNumbers;
            set => SetProperty(ref _invitedNumbers, value);
        }

        public InvitedNumberResponse InvitedNumber
        {
            get => _invitedNumber;
            set => SetProperty(ref _invitedNumber, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string SaveText
        {
            get => _saveText;
            set => SetProperty(ref _saveText, value);
        }


        public EventResponse Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            _organizer = JsonConvert.DeserializeObject<OrganizerResponse>(Settings.Organizer);
            if (parameters.ContainsKey("event"))
            {
                Event = parameters.GetValue<EventResponse>("event");
                IsEdit = true;
                Title = "Edit Event";
                SaveText = "Save";
            }
            else
            {
                Event = new EventResponse { Date = DateTime.Today };
                IsEdit = false;
                Title = "New Event";
                SaveText = "Select a room";
            }

            LoadEventTypesAsync();
            LoadInvitedNumbersAsync();
        }
        private async void LoadInvitedNumbersAsync()
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
                await _navigationService.GoBackAsync();
                return;
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.GetListAsync<InvitedNumberResponse>(
                url,
                "/api",
                "/InvitedNumbers/GetInvitedNumbers",
                "bearer",
                token.Token);

            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Getting the pet types list, please try later,",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var invitedNumbers = (List<InvitedNumberResponse>)response.Result;
            InvitedNumbers = new ObservableCollection<InvitedNumberResponse>(invitedNumbers);

            if (!string.IsNullOrEmpty(Event.InvitedsNumber.ToString()))
            {
                InvitedNumber = InvitedNumbers.FirstOrDefault(pt => pt.Value == Event.InvitedsNumber);
            }
        }

        private async void LoadEventTypesAsync()
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
                await _navigationService.GoBackAsync();
                return;
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.GetListAsync<EventTypeResponse>(
                url,
                "/api",
                "/EventTypes/GetEventTypes",
                "bearer",
                token.Token);

            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Getting the pet types list, please try later,",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var eventTypes = (List<EventTypeResponse>)response.Result;
            EventTypes = new ObservableCollection<EventTypeResponse>(eventTypes);

            if (!string.IsNullOrEmpty(Event.EventType))
            {
                EventType = EventTypes.FirstOrDefault(pt => pt.Name == Event.EventType);
            }
        }

        private async void SaveAsync()
        {
            var isValid = await ValidateData();
            if (!isValid)
            {
                return;
            }
            IsRunning = true;
            IsEnabled = false;


            if (IsEdit)
            {
                var request = new EventRequest
                {
                    Id = Event.Id,
                    Name = Event.Name,
                    InvitedsNumber = InvitedNumber.Value,
                    EventTypeId = EventType.Id,
                    OrganizerId = _organizer.Id,
                    Remarks = Event.Remarks,
                };
                var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
                var url = App.Current.Resources["UrlAPI"].ToString();
                var response = await _apiService.PutAsync(
                    url,
                    "api",
                    "/Event/EditEvent",
                    request,
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
                    "The event was updated susccesfully, please select the best room for you.",
                    "Accept");
                Settings.Event = JsonConvert.SerializeObject(request);
                Settings.EventDate = Event.Date.ToString();
                await EventsPageViewModel.GetInstance().RefreshUser();
            }
            else
            {
                var request = new EventRequest
                {
                    Name = Event.Name,
                    InvitedsNumber = InvitedNumber.Value,
                    EventTypeId = EventType.Id,
                    OrganizerId = _organizer.Id,
                    Remarks = Event.Remarks
                };
                var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
                var url = App.Current.Resources["UrlAPI"].ToString();
                var response = await _apiService.PostAsync(
                    url,
                    "api",
                    "/Event/AddEvent",
                    request,
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
                    "The event was added susccesfully, please select the best room for you.",
                    "Accept");

                Settings.Event = JsonConvert.SerializeObject(request);
                Settings.EventDate = Event.Date.ToString();
                await EventsPageViewModel.GetInstance().RefreshUser();
                await _navigationService.NavigateAsync("RoomsPage");
            }
        }

        private async Task<bool> ValidateData()
        {
            if (EventType is null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must select a Event type", "Accept");
                return false;
            }
            if (InvitedNumber is null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must select a range of guests", "Accept");
                return false;
            }
            if (string.IsNullOrEmpty(Event.Name))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a Name's Event.", "Accept");
                return false;
            }
            return true;
        }

    }
}
