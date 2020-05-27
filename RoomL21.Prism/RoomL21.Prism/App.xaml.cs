using Newtonsoft.Json;
using Prism;
using Prism.Ioc;
using RoomL21.Common.Helpers;
using RoomL21.Common.Models;
using RoomL21.Common.Services;
using RoomL21.Prism.ViewModels;
using RoomL21.Prism.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RoomL21.Prism
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjU4NTU5QDMxMzgyZTMxMmUzMFBQQm1XdUc5VGo2WS9QaVUxQ05lemEzUkxpUVg1Smw0QVZ6eDNjbXdQb2c9");

            InitializeComponent();

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            if (Settings.IsRemembered && token?.Expiration > DateTime.Now)
            {
                if (Settings.UserType == "Organizer")
                {
                    //await NavigationService.NavigateAsync("/L21MasterDetailPage/NavigationPage/EventsPage");
                    await NavigationService.NavigateAsync("/L21MasterDetailPage/NavigationPage/EventsPage");
                }
                else
                {
                    await NavigationService.NavigateAsync("/L21MasterDetailPage/NavigationPage/EventTabbedPage");
                }
            }
            else
            {
                await NavigationService.NavigateAsync("/NavigationPage/LoginPage");
                //await NavigationService.NavigateAsync("L21MasterDetailPage");
            }

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<RememberPasswordPage, RememberPasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<RoomL21MasterDetailPage, L21MasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<EventsPage, EventsPageViewModel>();
            containerRegistry.RegisterForNavigation<EventPage, EventPageViewModel>();
            containerRegistry.RegisterForNavigation<AgendaPage, AgendaPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
            containerRegistry.RegisterForNavigation<ProfilePage, ProfilePageViewModel>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage, ChangePasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<EditEvent, EditEventViewModel>();
            containerRegistry.RegisterForNavigation<RoomsPage, RoomsPageViewModel>();
            containerRegistry.RegisterForNavigation<AddInvitedsPage, AddInvitedsPageViewModel>();
            containerRegistry.RegisterForNavigation<RoomPage, RoomPageViewModel>();
            containerRegistry.RegisterForNavigation<EventTabbedPage, EventTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<InvitedsPage, InvitedsPageViewModel>();
        }
    }
}
