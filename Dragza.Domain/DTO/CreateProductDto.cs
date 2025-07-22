using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }
        public string? Preef { get; set; }
        public string? Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public Guid? ActiveIngredientId { get; set; }
        public IFormFile? Photo { get; set; }
        public string? ArabicName { get; set; }
    }
}
