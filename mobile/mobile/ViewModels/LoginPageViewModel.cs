// David Wahid
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using mobile.Models.HttpResponse;
using mobile.Options;
using mobile.Services;
using mobile.Views;
using MvvmHelpers.Commands;
using shared.Models.Account;
using shared.Utilities;
using Xamarin.Forms;

namespace mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public IAccountService mAccountService { get; }
        public IOptionsSnapshot<AccountOptions> mAccountOptions { get; }

        public LoginPageViewModel()
        {
            mAccountService = Startup.ServiceProvider.GetService<IAccountService>();
            mAccountOptions = Startup.ServiceProvider.GetService<IOptionsSnapshot<AccountOptions>>();

            //Email = mAccountOptions.Value.Email;
            //Password = mAccountOptions.Value.Password;
            IsValidName = true;
            IsValidEmail = true;
            IsValidPassword = true;
        }

        public LoginPageViewModel(
                    IAccountService accountService,
                    IOptionsSnapshot<AccountOptions> accountOptions)

        {
            mAccountService = accountService;
            mAccountOptions = accountOptions;
        }

        #region Properties
        private string name = string.Empty;
        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
                IsValidName = ValidateName(value);
            }
        }

        private string email = string.Empty;
        public string Email
        {
            get => email;
            set
            {
                SetProperty(ref email, value);
                IsValidEmail = RegexUtilities.IsValidEmail(value);
            }
        }

        private string password = string.Empty;
        public string Password
        {
            get => password;
            set
            {
                SetProperty(ref password, value);
                IsValidPassword = RegexUtilities.IsValidPassword(value);
            }
        }

        private bool isValidName = true;
        public bool IsValidName
        {
            get => isValidName;
            set => SetProperty(ref isValidName, value);
        }

        private bool isValidEmail = true;
        public bool IsValidEmail
        {
            get => isValidEmail;
            set => SetProperty(ref isValidEmail, value);
        }

        private bool isValidPassword = true;
        public bool IsValidPassword
        {
            get => isValidPassword;
            set => SetProperty(ref isValidPassword, value);
        }
        #endregion

        public ICommand RegisterCommand => new AsyncCommand(async () =>
        {
            await AccountAction(true);
        });

        public ICommand LoginCommand => new AsyncCommand(async () =>
        {
            await AccountAction();
        });

        async Task AccountAction(bool isRegister = false)
        {
            IsValidEmail = RegexUtilities.IsValidEmail(Email);
            IsValidPassword = RegexUtilities.IsValidPassword(Password);
            IsValidName = ValidateName(Name);

            if (IsValidEmail && IsValidPassword)
            {
                try
                {
                    Response<AccountResponseModel> response = null;

                    if (!isRegister)
                    {
                        response = await mAccountService.Login(Email, Password);
                    }
                    else if (IsValidName)
                    {
                        response = await mAccountService.Register(Email, Password, Name);
                    }

                    if (response == null)
                    {
                        throw new Exception("Unknow error occured! That's all we know...");
                    }

                    if (!response.IsSuccessful)
                    {
                        showError(response.Message);
                    }

                    saveUserInfo(response.ReturnObject);

                    await Application.Current.MainPage.Navigation.PushAsync(new Home());
                    IsErrorVisible = false;
                }
                catch (Exception e)
                {
                    showError(e.Message);
                    Debug.WriteLine(e.Message);
                }
            }
        }

        void showError(string message)
        {
            ErrorMessage = message;
            IsErrorVisible = true;
        }

        private bool ValidateName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) &&
                    name.Length <= 50 &&
                    name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        private void saveUserInfo(AccountResponseModel model)
        {
            Application.Current.Properties["logged-in"] = true;
            Application.Current.Properties["user-name"] = model.FullName;
            Application.Current.Properties["user-email"] = model.Email;
            Application.Current.Properties["user-role"] = model.Role;
            Application.Current.Properties["user-id"] = model.UserId;
            Application.Current.Properties["jwt-token"] = model.Token;
        }
    }
}
