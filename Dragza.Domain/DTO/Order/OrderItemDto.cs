using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO.Order
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string InventoryName { get; set; }
        public Guid ProductPriceId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid? InventoryUserId { get; set; }

    }
}
