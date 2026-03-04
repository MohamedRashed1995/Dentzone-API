using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Preef { get; set; }
        public string? ArabicPreef { get; set; }
        public string? Description { get; set; }
        public string? ArabicDescription { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Image { get; set; }
        public string? ArabicName { get; set; }
		public CategoryDto Category { get; set; }
        public List<ProductInventoryDto> Inventories { get; set; } = new();
        public List<ProductPriceResponseDto> Prices { get; set; } = new();
        public Guid? InventoryUserId { get; set; }
        public int? ProductCode { get; set; }
    }
}
