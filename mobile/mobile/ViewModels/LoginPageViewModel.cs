// David Wahid
using System.Windows.Input;
using mobile.Views;
using TinyMvvm;
using TinyNavigationHelper.Forms;

namespace mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public LoginPageViewModel()
        {
        }

        private string email = string.Empty;
        public string Email
        {
            get => email;
            set => Set(ref email, value);
        }

        private string password = string.Empty;
        public string Password
        {
            get => password;
            set => Set(ref password, value);
        }

        public ICommand LoginCommand => new TinyCommand(() =>
        {
            System.Console.WriteLine($"Email: {Email}\nPassword: {Password}");
        });

        public ICommand RegisterCommand => new TinyCommand(async () =>
        {
            await Navigation.NavigateToAsync(nameof(RegisterPage));
        });
    }
}
