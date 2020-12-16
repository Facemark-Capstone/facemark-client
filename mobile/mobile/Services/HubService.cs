// David Wahid
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using shared.Models.AI;

namespace mobile.Services
{
    public interface IHubService
    {
        delegate void OrderHandler(object sender, string orderId, string status, string connectionId, OrderResult result);
        delegate void ConnectionHandler(object sender, bool successful, string message);

        event OrderHandler OrderStatus;
        event ConnectionHandler Connected;
        event ConnectionHandler ConnectionFailed;
        bool IsConnected { get; set; }
        bool IsBusy { get; set; }
        string HubId { get; }

        Task ConnectAsync();
        Task DisconnectAsync();
    }
    public class HubService : IHubService
    {
        HubConnection connection;

        string hubId = string.Empty;

        public event IHubService.OrderHandler OrderStatus;
        public event IHubService.ConnectionHandler Connected;
        public event IHubService.ConnectionHandler ConnectionFailed;

        public bool IsConnected { get; set; }
        public bool IsBusy { get; set; }

        public string HubId => hubId;

        public HubService()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/aihub")
                .Build();
        }

        public async Task ConnectAsync()
        {
            try
            {
                IsBusy = true;

                connection.Closed += Connection_Closed;
                connection.On<string, string, string, OrderResult>("UpdateOrderStatus", OnOrderStatus);
                await connection.StartAsync();

                IsConnected = true;
                IsBusy = false;
                hubId = connection.ConnectionId;

                Connected?.Invoke(this, true, "Connection successful.");
            }
            catch (Exception ex)
            {
                ConnectionFailed?.Invoke(this, false, ex.Message);
                IsConnected = false;
                IsBusy = false;
            }
        }

        private void OnOrderStatus(string orderId, string status, string connectionId, OrderResult result)
        {
            OrderStatus?.Invoke(this, orderId, status, connectionId, result);
        }

        private Task Connection_Closed(Exception arg)
        {
            ConnectionFailed?.Invoke(this, false, arg.Message);
            IsConnected = false;
            IsBusy = false;
            return Task.CompletedTask;
        }

        public async Task DisconnectAsync()
        {
            await connection.StopAsync();
        }
    }
}
