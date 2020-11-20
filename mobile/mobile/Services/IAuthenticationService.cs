// David Wahid
using System.Threading.Tasks;

namespace mobile.Services
{
    public interface IAuthenticationService
    {
        Task InitializeAsync();
        string GetAccessToken();
    }
}
