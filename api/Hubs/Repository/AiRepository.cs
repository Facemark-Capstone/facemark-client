// David Wahid
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using shared.Models.Hub;

namespace api.Hubs.Repository
{
    public class AiRepository : IAiRepository
    {
        public IHubContext<AiHub> Hub { get; }

        public AiRepository(IHubContext<AiHub> hub)
        {
            Hub = hub;
        }

        public async Task UpdateOrderStatus(UpdateOrderStatusModel model)
        {
            await Hub.Clients.Client(model.HubConnectionId).SendAsync("UpdateOrderStatus", model.OrderId, model.Status.ToString(), model.HubConnectionId, model.Result);
        }
    }

    public interface IAiRepository
    {
        Task UpdateOrderStatus(UpdateOrderStatusModel model);
    }
}
