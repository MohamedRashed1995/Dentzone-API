using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO.ReturnOrder
{
    public class ReturnOrderDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid PharmacyUserId { get; set; }
        public string PharmacyName { get; set; }
        public DateTime RequestDate { get; set; }
        public ReturnOrderStatus Status { get; set; }
        public decimal TotalReturnValue { get; set; }
        public string? ReturnOrderNumber { get; set; }
        public Guid? InventoryUserId { get; set; }
        public virtual User? InventoryUser { get; set; }
        public List<ReturnedItemDto> Items { get; set; } = new();
    }
}
