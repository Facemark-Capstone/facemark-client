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

            if (Current.Properties.ContainsKey("logged-in") && (bool)Current.Properties["logged-in"])
            {
                MainPage = new NavigationPage(new Home());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
                Current.Properties["logged-in"] = false;
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
