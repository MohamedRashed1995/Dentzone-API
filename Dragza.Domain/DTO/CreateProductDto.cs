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
        //public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Preef { get; set; }
        public string? ArabicPreef { get; set; }
        public string? Description { get; set; }
        public string? ArabicDescription { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        //[Required]
        //public Guid InventoryUserId { get; set; }

        //public Guid? ActiveIngredientId { get; set; }
        public string? ImageName { get; set; }
        public IFormFile? Photo { get; set; }
        public string? ArabicName { get; set; }
    }
}
