// David Wahid
using MvvmHelpers;

namespace mobile.ViewModels
{
    public class ViewModelBase : BaseViewModel
    {
        public ViewModelBase()
        {
        }

        string errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        bool isErrorVisible = false;
        public bool IsErrorVisible
        {
            get => isErrorVisible;
            set => SetProperty(ref isErrorVisible, value);
        }
    }
}
