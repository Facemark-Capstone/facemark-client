// David Wahid
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using mobile.Models.HttpResponse;
using mobile.Options;
using shared.Models;
using shared.Models.Account;
using shared.Utilities;
using Xamarin.Essentials;

namespace mobile.Services
{
    public interface IAccountService
    {
        Task<Response<AccountResponseModel>> Login(string email, string password);
        Task<Response<AccountResponseModel>> Register(string fullName, string email, string password);
    }

    public class AccountService : IAccountService
    {
        public ILogger<AccountService> mLogger { get; }
        public IHttpClientFactory mHttpClient { get; }
        public IOptionsSnapshot<AccountOptions> mAccountOptions { get; }
        public IOptionsSnapshot<SecurityOptions> mSecurityOptions { get; }

        public AccountService(ILogger<AccountService> logger,
                IHttpClientFactory httpClient,
                IOptionsSnapshot<AccountOptions> accountOptions,
                IOptionsSnapshot<SecurityOptions> securityOptions)
        {
            mLogger = logger;
            mHttpClient = httpClient;
            mAccountOptions = accountOptions;
            mSecurityOptions = securityOptions;
        }


        public async Task<Response<AccountResponseModel>> Login(string email, string password)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                return new Response<AccountResponseModel>()
                {
                    IsSuccessful = false,
                    Message = "Http request failed. Check connectivity."
                };
            }

            var loginModel = new LoginModel()
            {
                Email = email,
                PasswordEncrypted = Aes128.EncryptToUrlSafeBase64String(password, mSecurityOptions.Value.AesEncryptionKey)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, mAccountOptions.Value.LoginUrl);
            request.Content = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");

            var client = mHttpClient.CreateClient();
            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await client.SendAsync(request);
            }
            catch (Exception e)
            {
                mLogger.LogError($"Http Request failed at: {nameof(AccountService)}");
                return new Response<AccountResponseModel>()
                {
                    IsSuccessful = false,
                    Message = "Http request failed. Check connectivity."
                };
            }

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                AccountResponseModel responseModel = await JsonSerializer.DeserializeAsync<AccountResponseModel>(responseStream, options);


                var response = new Response<AccountResponseModel>()
                {
                    StatusCode = (int)httpResponse.StatusCode,
                    IsSuccessful = true,
                    ReturnObject = responseModel
                };

                return response;
            }
            else
            {
                mLogger.LogError($"Http Request failed at: {nameof(AccountService)}");
                return new Response<AccountResponseModel>()
                {
                    IsSuccessful = false,
                    Message = $"Can't login... {httpResponse.StatusCode}",
                    StatusCode = (int)httpResponse.StatusCode,
                };
            }
        }

        public async Task<Response<AccountResponseModel>> Register(string fullName, string email, string password)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                return new Response<AccountResponseModel>()
                {
                    IsSuccessful = false,
                    Message = "Http request failed. Check connectivity."
                };
            }

            var userRequest = new UserRequest()
            {
                FullName = fullName,
                Email = email,
                PasswordEncrypted = Aes128.EncryptToUrlSafeBase64String(password, mSecurityOptions.Value.AesEncryptionKey),
                Role = EUserRole.Client
            };

            var request = new HttpRequestMessage(HttpMethod.Post, mAccountOptions.Value.RegisterUrl);
            request.Content = new StringContent(JsonSerializer.Serialize(userRequest), Encoding.UTF8, "application/json");

            var client = mHttpClient.CreateClient();
            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await client.SendAsync(request);

            }
            catch (Exception)
            {
                mLogger.LogError($"Http Request failed at: {nameof(AccountService)}");
                return new Response<AccountResponseModel>()
                {
                    IsSuccessful = false,
                    Message = "Http request failed. Check connectivity."
                };
            }

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                AccountResponseModel responseModel = await JsonSerializer.DeserializeAsync<AccountResponseModel>(responseStream, options);


                var response = new Response<AccountResponseModel>()
                {
                    StatusCode = (int)httpResponse.StatusCode,
                    IsSuccessful = true,
                    ReturnObject = responseModel
                };

                return response;
            }
            else
            {
                mLogger.LogError($"Http Request failed at: {nameof(AccountService)}");
                return new Response<AccountResponseModel>()
                {
                    IsSuccessful = false,
                    Message = $"Can't Register... {httpResponse.StatusCode}",
                    StatusCode = (int)httpResponse.StatusCode,
                };
            }

        }
    }
}
