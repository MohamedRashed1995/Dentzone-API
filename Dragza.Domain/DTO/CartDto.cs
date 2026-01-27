using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public double TotalAmountCart { get; set; }
        public ICollection<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
