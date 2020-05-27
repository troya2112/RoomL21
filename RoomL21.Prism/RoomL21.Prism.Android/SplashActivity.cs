
using Android.App;
using Android.OS;

namespace RoomL21.Prism.Droid
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Threading.Thread.Sleep(800);
            StartActivity(typeof(MainActivity));
        }
    }
}