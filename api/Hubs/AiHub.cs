// David Wahid
using System;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using shared.Models.Hub;

namespace api.Hubs
{
    public class AiHub : Hub
    {
        public ILogger<AiHub> mLogger { get; }
        public FacemarkDbContext mDbContext { get; }

        public AiHub(ILogger<AiHub> logger, FacemarkDbContext context)
        {
            mLogger = logger;
            mDbContext = context;
        }


        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async Task UpdateOrderStatus(Guid orderId)
        {
            var order = await mDbContext.Orders.Where(order => order.Id == orderId).FirstOrDefaultAsync();

            await Clients.Caller.SendAsync("UpdateOrderStatus", order?.OrderStatus, Context.ConnectionId);
        }
    }
}
