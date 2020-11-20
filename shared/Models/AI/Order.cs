// David Wahid
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shared.Models.AI
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        [Required]
        public byte[] ImageData { get; set; }

        [Required]
        public string DataHeaders { get; set; }

        [Required]
        public EOrderStatus OrderStatus { get; set; }

        [Required]
        public string HubConnectionId { get; set; }

        [Obsolete("Only used for model binding.")]
        public Order() { }

        public Order(string userId,
                DateTime createdAt,
                byte[] imageData,
                string dataHeaders,
                EOrderStatus orderStatus,
                string hubConnectionId)
        {
            UserId = userId;
            CreatedAt = createdAt;
            ImageData = imageData;
            DataHeaders = dataHeaders;
            OrderStatus = orderStatus;
            HubConnectionId = hubConnectionId;
        }
    }
}
