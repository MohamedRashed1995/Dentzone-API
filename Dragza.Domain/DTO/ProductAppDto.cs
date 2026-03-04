using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class ProductAppDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ArabicProductName { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string? ArabicDescription { get; set; }
        public string? Preef { get; set; }
        public string? ArabicPreef { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public Guid? InventoryUserId { get; set; }
    }
}
