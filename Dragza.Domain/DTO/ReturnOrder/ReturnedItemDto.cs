using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO.ReturnOrder
{
    public class ReturnedItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid ProductPriceId { get; set; }
        public int QuantityReturned { get; set; }
        public Guid ReasonId { get; set; }
        public string ReasonName { get; set; }
        public string? OtherReason { get; set; }
    }
}
