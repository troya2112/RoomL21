using Newtonsoft.Json;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomL21.Prism.ViewModels
{
    public class EventItemViewModel : EventResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectEventCommand;

        public EventItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectEventCommand => _selectEventCommand ?? (_selectEventCommand = new DelegateCommand(SelectEvent));

        private async void SelectEvent()
        {
            Settings.Event = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync("EventTabbedPage");
        }
    }
}
