// David Wahid
using System;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using mobile.Options;
using mobile.Services;
using mobile.Views;
using MvvmHelpers;
using MvvmHelpers.Commands;
using shared.Models.Account;
using shared.Utilities;
using Xamarin.Forms;

namespace mobile.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        public IAccountService mAccountService { get; }
        public IOptionsSnapshot<AccountOptions> mAccountOptions { get; }

        public LoginPageViewModel()
        {
            mAccountService = Startup.ServiceProvider.GetService<IAccountService>();
            mAccountOptions = Startup.ServiceProvider.GetService<IOptionsSnapshot<AccountOptions>>();

            Email = mAccountOptions.Value.Email;
            Password = mAccountOptions.Value.Password;
        }

        public LoginPageViewModel(
                    IAccountService accountService,
                    IOptionsSnapshot<AccountOptions> accountOptions)

        {
            mAccountService = accountService;
            mAccountOptions = accountOptions;
        }

        private string email = string.Empty;
        public string Email
        {
            get => email;
            set
            {
                SetProperty(ref email, value);
                IsValidEmail = true;
            }
        }

        private string password = string.Empty;
        public string Password
        {
            get => password;
            set
            {
                SetProperty(ref password, value);
                IsValidPassword = true;
            }
        }

        private bool isValidEmail = true;
        public bool IsValidEmail
        {
            get => isValidEmail;
            set
            {
                SetProperty(ref isValidEmail, value);
                if (!isValidEmail)
                {
                    EmailBackground = Color.FromHex("FF708F");
                }
                else
                {
                    EmailBackground = Color.FromHex("E0E1E9");
                }
            }
        }

        private bool isValidPassword = true;
        public bool IsValidPassword
        {
            get => isValidPassword;
            set
            {
                SetProperty(ref isValidPassword, value);
                if (!isValidPassword)
                {
                    PasswordBackground = Color.FromHex("FF708F");
                }
                else
                {
                    PasswordBackground = Color.FromHex("E0E1E9");
                }
            }
        }

        private Color emailBackground = Color.FromHex("E0E1E9");
        public Color EmailBackground
        {
            get => emailBackground;
            set => SetProperty(ref emailBackground, value);
        }

        private Color passwordBackground = Color.FromHex("E0E1E9");
        public Color PasswordBackground
        {
            get => passwordBackground;
            set => SetProperty(ref passwordBackground, value);
        }

        string errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        bool isError = false;
        public bool IsError
        {
            get => isError;
            set => SetProperty(ref isError, value);
        }

        public ICommand LoginCommand => new AsyncCommand(async () =>
        {
            IsValidEmail = RegexUtilities.IsValidEmail(Email);
            IsValidPassword = RegexUtilities.IsValidPassword(Password);

            if (IsValidEmail && IsValidPassword)
            {
                try
                {
                    var response = await mAccountService.Login(Email, Password);
                    if (response == null)
                    {
                        ErrorMessage = "Unknown error occured! Check connectivity...";
                        IsError = true;
                        return;
                    }

                    if (!response.IsSuccessful)
                    {
                        ErrorMessage = response.Message;
                        IsError = true;
                        return;
                    }

                    saveUserInfo(response.ReturnObject);

                    await Application.Current.MainPage.Navigation.PushAsync(new Home());
                    IsError = false;
                }
                catch (Exception e)
                {
                    ErrorMessage = "Unknown error occured! Check connectivity...";
                    IsError = true;
                    Debug.WriteLine(e.Message);
                }
            }
        });

        private void saveUserInfo(AccountResponseModel model)
        {
            //Application.Current.Properties["logged-in"] = true;
            Application.Current.Properties["user-name"] = model.FullName;
            Application.Current.Properties["user-email"] = model.Email;
            Application.Current.Properties["user-role"] = model.Role;
            Application.Current.Properties["user-id"] = model.UserId;
            //Application.Current.Properties["jwt-token"] = model.Token;
        }
    }
}
