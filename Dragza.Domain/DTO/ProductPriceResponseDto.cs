using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
	//public class ProductPriceResponseDto
	//{
	//    public Guid Id { get; set; }
	//    public Guid ProductId { get; set; }
	//    public string ProductName { get; set; }
	//    public string? ProductArabicName { get; set; }
	//    public Guid CategoryId { get; set; }
	//    public string CategoryName { get; set; }
	//    public decimal PurchasePrice { get; set; }
	//    public decimal SalesPrice { get; set; }
	//    public DateTime CreationDate { get; set; }
	//    public Guid InventoryUserId { get; set; }
	//    public string InventoryUserName { get; set; }
	//    public int StockQuantity { get; set; }
	//}
	public class ProductPriceResponseDto
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string ProductName { get; set; }
		public string? ProductArabicName { get; set; }

		// Added Product details
		public string? Preef { get; set; }
		public string? Description { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string? Image { get; set; }
		public CategoryDto Category { get; set; }
		public ActiveIngredientDto? ActiveIngredient { get; set; }

		// Existing price-related fields
		public Guid CategoryId { get; set; }
		public string CategoryName { get; set; }
		public decimal PurchasePrice { get; set; }
		public decimal SalesPrice { get; set; }
		public DateTime CreationDate { get; set; }
		public Guid InventoryUserId { get; set; }
		public string InventoryUserName { get; set; }
		public int StockQuantity { get; set; }
        public decimal DiscountRate { get; set; }



    }
}
