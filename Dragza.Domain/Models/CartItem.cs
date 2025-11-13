using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal? TotalAmount { get; set; }
        public Guid ProductPriceId { get; set; }
        [ForeignKey("ProductPriceId")]
        public ProductPrice ProductPrice { get; set; }

        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart Cart { get; set; } = null!;

        public Guid InventoryUserId { get; set; }
        [ForeignKey("InventoryUserId")]
        public  User InventoryUser { get; set; } = null!;
    }
}
