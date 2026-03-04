using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class ProductInventoryDto
    {
        public Guid InventoryUserId { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public int StockQuantity { get; set; }
    }
}
