using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoomL21.Prism.ViewModels
{
    public class AgendaPageViewModel : ViewModelBase
    {
        public AgendaPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "My Agenda";
        }
    }
}
