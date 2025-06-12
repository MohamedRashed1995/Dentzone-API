using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class InventoryUserPriceDetailsDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductPriceId { get; set; }
        public string ProductName { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StockQuantity { get; set; }

    }
}
