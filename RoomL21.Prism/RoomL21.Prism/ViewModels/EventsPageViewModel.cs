using Newtonsoft.Json;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using RoomL21.Common.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RoomL21.Prism.ViewModels
{
    public class EventsPageViewModel : ViewModelBase
    {
        private OrganizerResponse _organizer;
        private InvitedResponse _invited;
        private ObservableCollection<EventItemViewModel> _events;
        private bool _isRunning;
        private DelegateCommand _addEventCommand;
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private static EventsPageViewModel _instance;

        public EventsPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            _apiService = apiService;
            LoadUser();
            IsRunning = false;
        }

        public static EventsPageViewModel GetInstance()
        {
            return _instance;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public DelegateCommand AddEventCommand => _addEventCommand ?? (_addEventCommand = new DelegateCommand(AddEvent));

        public ObservableCollection<EventItemViewModel> Events
        {
            get => _events;
            set => SetProperty(ref _events, value);
        }

        private void LoadUser()
        {
            if (Settings.UserType == "Organizer")
            {
                _organizer = JsonConvert.DeserializeObject<OrganizerResponse>(Settings.Organizer);
                Title = $"Events of: {_organizer.FirstName}";
                Events = new ObservableCollection<EventItemViewModel>(_organizer.Events.Select(p => new EventItemViewModel(_navigationService)
                {
                    InvitedsNumber = p.InvitedsNumber,
                    Id = p.Id,
                    Name = p.Name,
                    EventType = p.EventType,
                    Date = p.Date,
                    Remarks = p.Remarks,
                    EventTypeId = p.EventTypeId
                }).ToList());
            }
            else
            {
                _invited = JsonConvert.DeserializeObject<InvitedResponse>(Settings.Invited);
                Title = $"Events of: {_organizer.FirstName}";
                Events = new ObservableCollection<EventItemViewModel>(_organizer.Events.Select(p => new EventItemViewModel(_navigationService)
                {
                    InvitedsNumber = p.InvitedsNumber,
                    Id = p.Id,
                    Name = p.Name,
                    EventType = p.EventType,
                    Date = p.Date,
                    Remarks = p.Remarks,
                }).ToList());
            }
        }

        public async Task RefreshUser()
        {
            IsRunning = true;
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var url = App.Current.Resources["UrlAPI"].ToString();
            if (Settings.UserType == "Organizer")
            {
                var response = await _apiService.GetOrganizerByEmailAsync(url, "api", "/Organizers/GetOrganizerByEmail", "bearer", token.Token, _organizer.Email);
                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    await App.Current.MainPage.DisplayAlert("Error", "You did select an incorrect user type", "Accept");
                    return;
                }

                var organizer = response.Result;
                Settings.Organizer = JsonConvert.SerializeObject(organizer);
            }
            else
            {
                var response = await _apiService.GetInvitedByEmailAsync(url, "api", "/Inviteds/GetInvitedByEmail", "bearer", token.Token, _invited.Email);
                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    await App.Current.MainPage.DisplayAlert("Error", "You did select an incorrect user type", "Accept");
                    return;
                }

                var invited = response.Result;
                Settings.Invited = JsonConvert.SerializeObject(invited);
            }
            LoadUser();
            IsRunning = false;

        }

        private async void AddEvent()
        {
            await _navigationService.NavigateAsync("EditEvent");
        }
    }
}
