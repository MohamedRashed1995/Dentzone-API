using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class PopularProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ArabicDescription { get; set; }
        public string? Preef { get; set; }
        public string? ArabicPreef { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? DiscountRate { get; set; }
        public decimal? SalesPrice { get; set; }
        public int StockQuantity { get; set; }

        // Inventory info
        //public Guid InventoryUserId { get; set; }
        //public string InventoryFullName { get; set; } = string.Empty;
        //public string InventoryEmail { get; set; } = string.Empty;
        //public string InventoryPhone { get; set; } = string.Empty;
    }
}
