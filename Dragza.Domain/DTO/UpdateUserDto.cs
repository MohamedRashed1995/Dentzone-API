using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UpdateUserDto
    {
        public string? BussinesName { get; set; }

        public bool IsPharmacy { get; set; }

        public IFormFile? Photo { get; set; }

        public bool? IsActive { get; set; }

        public string? UserName { get; set; }
        public string? Address { get; set; }

        public string? NomalizedUserName { get; set; }

        public string? Email { get; set; }

        public bool? EmailConfirmed { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        public bool? PhoneConfirmed { get; set; }

        public string? RegionName { get; set; } = null!;
        public string? DesName { get; set; } = null!;
        public Decimal? MinOrder { get; set; } = null!;

        public string? Lang { get; set; }

        public string? Lat { get; set; }

        public Guid? City { get; set; }
        public Guid? GovId { get; set; }
        public Guid? RegionId { get; set; }
        public PharmacyDetailsDto? PharmacyDetails { get; set; }
    }
}
