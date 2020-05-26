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
    public class RoomItemViewModel : RoomResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectRoomCommand;

        public RoomItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectRoomCommand => _selectRoomCommand ?? (_selectRoomCommand = new DelegateCommand(SelectRoom));

        private async void SelectRoom()
        {
            Settings.Room = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync("RoomPage");
        }
    }
}
