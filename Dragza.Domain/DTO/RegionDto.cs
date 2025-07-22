using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class RegionDto
    {
        public Guid Id { get; set; }
        public string RegionName { get; set; }
        public string Lang { get; set; }
        public string Lat { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
