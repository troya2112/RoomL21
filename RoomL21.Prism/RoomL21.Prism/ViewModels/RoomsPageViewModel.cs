using Newtonsoft.Json;
using Prism.Navigation;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using RoomL21.Common.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RoomL21.Prism.ViewModels
{
    public class RoomsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private EventResponse _event;
        private ObservableCollection<RoomItemViewModel> _rooms;
        private bool _isRunning;
        public RoomsPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Select a room";
            IsRunning = false;
        }
        public ObservableCollection<RoomItemViewModel> Rooms
        {
            get => _rooms;
            set => SetProperty(ref _rooms, value);
        }
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            _event = JsonConvert.DeserializeObject<EventResponse>(Settings.Event);
            LoadRoomsAsync();
        }

        private async void LoadRoomsAsync()
        {
            IsRunning = true;
            var url = App.Current.Resources["UrlAPI"].ToString();

            var connection = await _apiService.CheckConnection(url);
            if (!connection)
            {
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Check the internet connection.",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.GetListAsync<RoomResponse>(
                url,
                "/api",
                "/Rooms/GetRooms",
                "bearer",
                token.Token);

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Getting the room list, please try later,",
                    "Accept");
                await _navigationService.GoBackAsync();
                return;
            }
            IsRunning = false;
            var rooms = (List<RoomResponse>)response.Result;
            Rooms = new ObservableCollection<RoomItemViewModel>(rooms.Select(r => new RoomItemViewModel(_navigationService)
            {
                Owner = r.Owner,
                Id = r.Id,
                Capacity = r.Capacity,
                Address = r.Address
            }).Where(r => (r.Capacity >= _event.InvitedsNumber)).ToList());
        }
    }
}
