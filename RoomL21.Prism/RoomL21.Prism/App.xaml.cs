using Prism;
using Prism.Ioc;
using RoomL21.Common.Services;
using RoomL21.Prism.ViewModels;
using RoomL21.Prism.Views;
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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjM4MzI4QDMxMzgyZTMxMmUzMFJFWkRzdVZydG5WcVZjeUdoaUJjZi81a0xvb3VXTCsvTTVPOEkyQ2JxNnM9");
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<L21MasterDetailPage, L21MasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<AddInvitedsPage, AddInvitedsPageViewModel>();
            containerRegistry.RegisterForNavigation<AgendaPage, AgendaPageViewModel>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage, ChangePasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<EditEvent, EditEventViewModel>();
            containerRegistry.RegisterForNavigation<EventPage, EventPageViewModel>();
            containerRegistry.RegisterForNavigation<EventsPage, EventsPageViewModel>();
            containerRegistry.RegisterForNavigation<EventTabbedPage, EventTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<InvitedsPage, InvitedsPageViewModel>();
            containerRegistry.RegisterForNavigation<MapPage, MapPageViewModel>();
            containerRegistry.RegisterForNavigation<ProfilePage, ProfilePageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<RememberPasswordPage, RememberPasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<RoomPage, RoomPageViewModel>();
            containerRegistry.RegisterForNavigation<RoomsPage, RoomsPageViewModel>();
        }
    }
}
