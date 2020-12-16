// David Wahid
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace mobile.Services
{
    public interface IAppService
    {
        void SaveProperties(Dictionary<string, object> properties);
        T GetProperty<T>(string id);
        void SetProperty<T>(string key, T value);
        bool HasProperty(string id);
        Task ShowAlert(string title, string message, string action);
        Task NavigateToAsync(Page page);
    }

    public class AppService : IAppService
    {
        public Application mApp { get; }

        public AppService()
        {
            mApp = Application.Current;
        }


        public T GetProperty<T>(string id)
        {
            return (T)mApp.Properties[id];
        }

        public void SetProperty<T>(string key, T value)
        {
            mApp.Properties[key] = value;
        }

        public bool HasProperty(string id)
        {
            return mApp.Properties.ContainsKey(id);
        }

        public void SaveProperties(Dictionary<string, object> properties)
        {
            foreach (var p in properties)
            {
                mApp.Properties[p.Key] = p.Value;
            }
        }

        public async Task ShowAlert(string title, string message, string action)
        {
            await mApp.MainPage.DisplayAlert(title, message, action);
        }

        public async Task NavigateToAsync(Page page)
        {
            if (page != null)
            {
                await mApp.MainPage.Navigation.PushAsync(page);
            }
        }
    }
}
