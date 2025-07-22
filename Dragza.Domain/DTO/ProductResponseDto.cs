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
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Image { get; set; }
        public string? ArabicName { get; set; }
        public CategoryDto Category { get; set; }
        public ActiveIngredientDto? ActiveIngredient { get; set; }
        public List<ProductPriceResponseDto> Prices { get; set; } = new();
    }
}
