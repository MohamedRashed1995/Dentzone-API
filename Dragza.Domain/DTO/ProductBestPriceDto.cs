using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class ProductBestPriceDto
    {
        public Guid ProductId { get; set; }
        public Guid PriceId { get; set; }
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductArabicDescription { get; set; }
        public string? ProductPreef { get; set; }
        public string? ProductArabicPreef{ get; set; }
        public string? ProductImageUrl { get; set; }
        public decimal? BestSalesPrice { get; set; }
        //public DateTime PriceDate { get; set; }
        public UserDto InventoryUser { get; set; }
        public string? ProductArabicName { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }

    }
}
