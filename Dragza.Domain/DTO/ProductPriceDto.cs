using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class ProductPriceDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

      

        public decimal? PurchasePrice { get; set; }

        public decimal? SalesPrice { get; set; }

        public DateTime? CreationDate { get; set; }

      
    

        public Guid InventoryUserId { get; set; }
        public int StockQuantity { get; set; }
        public int MaxQuantity { get; set; }

    }
}
