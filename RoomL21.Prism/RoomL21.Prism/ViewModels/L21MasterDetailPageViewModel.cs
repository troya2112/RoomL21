using Prism.Navigation;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RoomL21.Prism.ViewModels
{
    public class L21MasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public L21MasterDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            LoadMenus();
        }
        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        //TODO tener en cuenta el rol
        private void LoadMenus()
        {
            if (Settings.UserType == "Organizer")
            {
                var menus = new List<Menu>
                {
                    new Menu
                    {

                        Icon = "ic_event_note",
                        PageName = "EventsPage",
                        Title = "My Events"
                    },

                    new Menu
                    {
                        Icon = "ic_person",
                        PageName = "ProfilePage",
                        Title = "Modify Profile"
                    },

                    new Menu
                    {
                        Icon = "ic_exit_to_app",
                        PageName = "LoginPage",
                        Title = "Logout"
                    }
                };
                Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title
                }).ToList());
            }
            else
            {
                var menus = new List<Menu>
                {
                    new Menu
                    {

                        Icon = "ic_event_note",
                        PageName = "EventTabbedPage",
                        Title = "My Event"
                    },

                    new Menu
                    {
                        Icon = "ic_person",
                        PageName = "ProfilePage",
                        Title = "Modify Profile"
                    },

                    new Menu
                    {
                        Icon = "ic_exit_to_app",
                        PageName = "LoginPage",
                        Title = "Logout"
                    }
                };
                Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title
                }).ToList());
            }

        }
    }
}
