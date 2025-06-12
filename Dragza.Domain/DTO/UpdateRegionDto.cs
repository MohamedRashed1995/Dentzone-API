using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UpdateRegionDto
    {
        [Required]
        [StringLength(100)]
        public string RegionName { get; set; }

        [StringLength(50)]
        public string Lang { get; set; }

        [StringLength(50)]
        public string Lat { get; set; }
    }
}
