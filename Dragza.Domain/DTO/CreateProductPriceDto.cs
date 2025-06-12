using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CreateProductPriceDto
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }
        public int StockQuantity { get; set; }
    }
}
