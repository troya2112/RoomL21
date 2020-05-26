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
    public class InvitedsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private ObservableCollection<InvitedItemViewModel> _inviteds;
        private int _invitedsNumber;
        private bool _isRunning;
        public InvitedsPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Guests List";
            LoadInviteds();
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            LoadInviteds();
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }
        public ObservableCollection<InvitedItemViewModel> Inviteds
        {
            get => _inviteds;
            set => SetProperty(ref _inviteds, value);
        }

        private async void LoadInviteds()
        {
            IsRunning = true;
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var url = App.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.GetListAsync<InvitedResponse>(
            url,
            "/api",
            "/Inviteds/GetInviteds",
            "bearer",
            token.Token);
            IsRunning = false;
            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }
            var eventVar = JsonConvert.DeserializeObject<EventResponse>(Settings.Event);
            var inviteds = (List<InvitedResponse>)response.Result;
            _invitedsNumber = inviteds.Where(i => i.EventId == eventVar.Id).Count();
            Title = $"Guests List ({_invitedsNumber})";
            Inviteds = new ObservableCollection<InvitedItemViewModel>(inviteds.Where(i => i.EventId == eventVar.Id).Select(p => new InvitedItemViewModel(_navigationService)
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address
            }).ToList());
        }
    }
}
