using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UserWithPharmacyDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Guid RegionId { get; set; }
        public string RegionName { get; set; }
        public bool IsPharmacy { get; set; }
        public Guid? SubAreaId { get; set; }
        public string? SubAreaName { get; set; }
        public PharmacyDetailsResponseDto? PharmacyDetails { get; set; }
    }
}
