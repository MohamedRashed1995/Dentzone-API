using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CartItemDto
    {
        public int Id { get; set; }

        public Guid ProductId { get; set; }
   
        public ProductDtoCart Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal? TotalAmount { get; set; }
        public Guid ProductPriceId { get; set; }
        public int status { get; set; } = 0;

        public ProductPriceDto ProductPrice { get; set; }
        public Guid InventoryUserId { get; set; }
       
        public UserDtoCart InventoryUser { get; set; } = null!;
    }
}
