using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using RoomL21.Common.Services;

namespace RoomL21.Prism.ViewModels
{
    public class RoomPageViewModel : ViewModelBase
    {
        private RoomResponse _room;
        private EventResponse _event;
        private string _eventDate;
        private DelegateCommand _selectRoomCommand;
        private bool _isRunning;
        private bool _isEnabled;
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        public RoomPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Room Details";
            IsRunning = false;
            IsEnabled = true;

        }
        public RoomResponse Room
        {
            get => _room;
            set => SetProperty(ref _room, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Room = JsonConvert.DeserializeObject<RoomResponse>(Settings.Room);
            _event = JsonConvert.DeserializeObject<EventResponse>(Settings.Event);
            _eventDate = Settings.EventDate;
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

        public DelegateCommand SelectRoomCommand => _selectRoomCommand ?? (_selectRoomCommand = new DelegateCommand(SelectRoomAsync));

        private async void SelectRoomAsync()
        {
            IsRunning = true;
            IsEnabled = false;
            var request = new EventRequest
            {
                Id = _event.Id,
                Name = _event.Name,
                InvitedsNumber = _event.InvitedsNumber,
                EventTypeId = _event.EventTypeId,
                OrganizerId = _event.Id,
                Remarks = _event.Remarks,
                RoomId = Room.Id
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
            "The room was selected susccesfully, please add your guests.",
            "Accept");
            Settings.Event = JsonConvert.SerializeObject(request);
            await EventsPageViewModel.GetInstance().RefreshUser();
            await _navigationService.NavigateAsync("AddInvitedsPage");
        }
    }
}
