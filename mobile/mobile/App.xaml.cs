using mobile.Views;
using Xamarin.Forms;

namespace mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Device.SetFlags(new string[] { "Expander_Experimental" });

            Startup.Init();

            if (Current.Properties.ContainsKey("logged-in") && (bool)Current.Properties["logged-in"])
            {
                MainPage = new NavigationPage(new Home());
            }
            else
            {
                Current.Properties["logged-in"] = false;
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
