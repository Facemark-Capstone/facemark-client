// David Wahid
using System;
using shared.Models.AI;

namespace shared.Models.Analysis
{
    public class PlaceOrderResponse
    {
        public string OrderId { get; set; }
        public EOrderStatus Status { get; set; }
    }
}
