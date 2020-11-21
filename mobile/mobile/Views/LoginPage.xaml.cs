// David Wahid
using Xamarin.Forms;

namespace mobile.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            registerCard.TranslationY = 600;
        }

        async void RevealRegisterCardAsync(object sender, System.EventArgs e)
        {
            await loginCard.TranslateTo(0, 600, 250, Easing.SinIn);
            loginCard.IsVisible = false;
            registerCard.IsVisible = true;
            await registerCard.TranslateTo(0, 0, 250, Easing.SinOut);
        }

        async void RevealLoginCardAsync(object sender, System.EventArgs e)
        {
            await registerCard.TranslateTo(0, 600, 250, Easing.SinIn);
            registerCard.IsVisible = false;
            loginCard.IsVisible = true;
            await loginCard.TranslateTo(0, 0, 250, Easing.SinOut);
        }
    }
}
