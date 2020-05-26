using Newtonsoft.Json;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoomL21.Prism.ViewModels
{
    public class EventPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private EventResponse _event;
        private DelegateCommand _editEventCommand;
        private DelegateCommand _selectRoomCommand;
        private DelegateCommand _addInvitedsCommand;
        private bool _isVisible;
        public EventPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            if (Settings.UserType=="Organizer")
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }
            _navigationService = navigationService;
            Title = "Event Details";
        }
        public DelegateCommand EditEventCommand => _editEventCommand ?? (_editEventCommand = new DelegateCommand(EditEventAsync));

        public DelegateCommand SelectRoomCommand => _selectRoomCommand ?? (_selectRoomCommand = new DelegateCommand(SelectRoom));
        public DelegateCommand AddInvitedsCommand => _addInvitedsCommand ?? (_addInvitedsCommand = new DelegateCommand(AddInviteds));

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }
        public EventResponse Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Event = JsonConvert.DeserializeObject<EventResponse>(Settings.Event);
        }

        private async void SelectRoom()
        {
            await _navigationService.NavigateAsync("RoomsPage");
        }

        private async void AddInviteds()
        {
            await _navigationService.NavigateAsync("AddInvitedsPage");
        }

        private async void EditEventAsync()
        {
            var parameters = new NavigationParameters
            {
                { "event", Event }
            };

            await _navigationService.NavigateAsync("EditEvent", parameters);
        }
    }
}
