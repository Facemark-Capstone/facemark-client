// David Wahid
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using mobile.Models.HttpResponse;
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
    public class RegisterPageViewModel : BaseViewModel
    {
        public IAccountService mAccountService { get; }
        public IOptionsSnapshot<AccountOptions> mAccountOptions { get; }

        public RegisterPageViewModel()
        {
            mAccountService = Startup.ServiceProvider.GetService<IAccountService>();
            mAccountOptions = Startup.ServiceProvider.GetService<IOptionsSnapshot<AccountOptions>>();

            Password = mAccountOptions.Value.Password;
            Email = mAccountOptions.Value.Email;
            Name = mAccountOptions.Value.Name;

        }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
                IsValidName = true;
            }
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

        private bool isValidName = true;
        public bool IsValidName
        {
            get => isValidName;
            set
            {
                SetProperty(ref isValidName, value);
                if (!isValidName)
                {
                    NameBackground = Color.FromHex("FF708F");
                }
                else
                {
                    NameBackground = Color.FromHex("E0E1E9");
                }
            }
        }

        private Color nameBackground = Color.FromHex("E0E1E9");
        public Color NameBackground
        {
            get => nameBackground;
            set => SetProperty(ref nameBackground, value);
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

        public ICommand RegisterCommand => new AsyncCommand(async () =>
        {
            IsValidEmail = RegexUtilities.IsValidEmail(Email);
            IsValidPassword = RegexUtilities.IsValidPassword(Password);
            IsValidName = ValidateName(Name);

            if (IsValidEmail && IsValidPassword && IsValidName)
            {
                try
                {
                    Response<shared.Models.Account.AccountResponseModel> response = await mAccountService.Register(Name, Email, Password);
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
            Application.Current.Properties["logged-in"] = true;
            Application.Current.Properties["user-name"] = model.FullName;
            Application.Current.Properties["user-email"] = model.Email;
            Application.Current.Properties["user-role"] = model.Role;
            Application.Current.Properties["user-id"] = model.UserId;
            Application.Current.Properties["jwt-token"] = model.Token;
        }

        private bool ValidateName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) &&
                    name.Length <= 50 &&
                    name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }
    }
}
