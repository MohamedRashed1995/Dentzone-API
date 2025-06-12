using Dragza.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO.Order
{
    public class CreateOrderDto
    {
        public Guid? CouponId { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; } // "Cash", "Credit", or "Mixed"
        public decimal? CreditAmount { get; set; }
    }
}
