using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class GovernateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? RegionId { get; set; }
        public string RegionName { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
