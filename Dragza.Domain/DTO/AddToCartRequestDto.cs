using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class AddToCartRequestDto
    {
        public string UserId { get; set; } = null!;
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
