using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Preef { get; set; }
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ActiveIngredientId { get; set; }
    }
}
