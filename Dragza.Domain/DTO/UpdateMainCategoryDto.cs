using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UpdateMainCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? ArabicName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
