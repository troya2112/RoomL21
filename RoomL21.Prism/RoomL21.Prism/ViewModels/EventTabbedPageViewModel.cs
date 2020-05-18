using Newtonsoft.Json;
using Prism.Navigation;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;

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
