using Dragza.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid PharmacyUserId { get; set; }
        public Guid? InventoryUserId { get; set; }
        public Guid? CouponId { get; set; }
        public decimal? DescountAmount { get; set; }
        public string DescountType { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? DeliverDate { get; set; }
        public string PharmacyName { get; set; }
        public string InventoryName { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
