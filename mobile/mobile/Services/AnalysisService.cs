// David Wahid
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using mobile.Errors;
using mobile.Exceptions;
using mobile.Models;
using mobile.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using shared.Models.Analysis;

namespace mobile.Services
{
    public interface IAnalysisService
    {
        Task<PlaceOrderResponse> PlaceOrder(Stream imageStream, string hubId, string userId, string token);
        Task<OrderResponse> GetResult(string hubId, string userId, string token, string orderId);
    }

    public class AnalysisService : IAnalysisService
    {
        IHttpClientFactory mClient;
        IOptionsSnapshot<AnalysisOptions> mOptions { get; }

        static JsonSerializerSettings s_settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public AnalysisService(IOptionsSnapshot<AnalysisOptions> options, IHttpClientFactory client)
        {
            mOptions = options;
            mClient = client;
        }

        public async Task<PlaceOrderResponse> PlaceOrder(Stream imageStream, string hubId, string userId, string token)
        {
            var client = mClient.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $" {token}");
            client.DefaultRequestHeaders.Add("hub-id", hubId);
            client.DefaultRequestHeaders.Add("user-id", userId);

            var requestUrl = mOptions.Value.AnalyzeUrl;
            return await SendRequestAsync<Stream, PlaceOrderResponse>(client, HttpMethod.Post, requestUrl, imageStream);
        }

        public async Task<OrderResponse> GetResult(string hubId, string userId, string token, string orderId)
        {
            var client = mClient.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $" {token}");
            client.DefaultRequestHeaders.Add("hub-id", hubId);
            client.DefaultRequestHeaders.Add("user-id", userId);
            client.DefaultRequestHeaders.Add("order-id", orderId);

            var requestUrl = mOptions.Value.AnalyzeUrl;
            return await SendRequestAsync<Stream, OrderResponse>(client, HttpMethod.Get, requestUrl, null);
        }

        async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpClient client, HttpMethod httpMethod, string requestUrl, TRequest requestBody)
        {
            var request = new HttpRequestMessage(httpMethod, requestUrl);
            request.RequestUri = new Uri(requestUrl);
            if (requestBody != null)
            {
                if (requestBody is Stream)
                {
                    request.Content = new StreamContent(requestBody as Stream);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                }
                else
                {
                    // If the image is supplied via a URL
                    request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, s_settings), Encoding.UTF8, "application/json");
                }
            }

            HttpResponseMessage responseMessage = await client.SendAsync(request);
            if (responseMessage.IsSuccessStatusCode)
            {
                string responseContent = null;
                if (responseMessage.Content != null)
                {
                    responseContent = await responseMessage.Content.ReadAsStringAsync();
                }
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    return JsonConvert.DeserializeObject<TResponse>(responseContent, s_settings);
                }
                return default(TResponse);
            }
            else
            {
                if (responseMessage.Content != null && responseMessage.Content.Headers.ContentType.MediaType.Contains("application/json"))
                {
                    string error = await responseMessage.Content.ReadAsStringAsync();
                    ClientError ex = JsonConvert.DeserializeObject<ClientError>(error);
                    if (ex.Error != null)
                    {
                        throw new AnalysisException(ex.Error.ErrorCode, ex.Error.Message, responseMessage.StatusCode);
                    }
                    else
                    {
                        ServiceError serviceEx = JsonConvert.DeserializeObject<ServiceError>(error);
                        if (ex != null)
                        {
                            throw new AnalysisException(serviceEx.ErrorCode, serviceEx.Message, responseMessage.StatusCode);
                        }
                        else
                        {
                            throw new AnalysisException("Unknown", "Unknown Error", responseMessage.StatusCode);
                        }
                    }
                }
                responseMessage.EnsureSuccessStatusCode();
            }
            return default(TResponse);
        }
    }
}
