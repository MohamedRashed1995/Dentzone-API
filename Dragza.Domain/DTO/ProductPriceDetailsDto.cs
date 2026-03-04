using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class ProductPriceDetailsDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductPriceId { get; set; }
        public string? ImageName { get; set; }
        public string ProductName { get; set; }
        public string? ArabicProductName { get; set; }
        public string? Description { get; set; }
        public string? ArabicDescription { get; set; }
        public string? Preef {  get; set; }
        public string? ArabicPreef { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }
        public DateTime CreationDate { get; set; }
        public CategoryDto Category { get; set; }
        public Guid InventoryUserId { get; set; }
        public UserDto InventoryUser { get; set; }
        public int StockQuantity { get; set; }
        public decimal DiscountRate { get; set; }

    }
}
