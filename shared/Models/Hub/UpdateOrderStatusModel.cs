// David Wahid
using System;
using shared.Models.AI;

namespace shared.Models.Hub
{
    public class UpdateOrderStatusModel
    {
        public Guid OrderId { get; set; }
        public EOrderStatus Status { get; set; }
        public string HubConnectionId { get; set; }
        public OrderResult Result { get; }

        [Obsolete("Only used for model binding.")]
        public UpdateOrderStatusModel() { }

        public UpdateOrderStatusModel(Guid orderId, EOrderStatus status, string hubConnectionId, OrderResult result)
        {
            OrderId = orderId;
            Status = status;
            HubConnectionId = hubConnectionId;
            Result = result;
        }
    }
}
