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
    public class EventTabbedPageViewModel : ViewModelBase
    {
        private EventResponse _event;
        public EventTabbedPageViewModel(INavigationService navigationService) : base(navigationService)
        {

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
            Title = Event.Name;
        }
    }
}
