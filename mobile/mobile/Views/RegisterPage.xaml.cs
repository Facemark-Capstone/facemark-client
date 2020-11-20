// David Wahid
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace mobile.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        async void NavigateBackToLogin(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }
    }
}
