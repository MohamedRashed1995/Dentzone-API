using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class ReturnReasonDto
    {
        public Guid Id { get; set; }
        public string Reason { get; set; } = null!;
    }

    public class CreateReturnReasonDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Reason { get; set; } = null!;
    }

    public class UpdateReturnReasonDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Reason { get; set; } = null!;
    }
}
