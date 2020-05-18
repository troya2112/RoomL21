using Prism.Navigation;

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
