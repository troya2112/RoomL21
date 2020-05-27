using Prism.Navigation;

namespace RoomL21.Prism.ViewModels
{
    public class MapPageViewModel : ViewModelBase
    {
        public MapPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "My map";
        }
    }
}
