using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public bool IsPharmacy { get; set; }    = false;
        public string Region { get; set; } = string.Empty;
        public decimal MinOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid Accountid { get; set; }

        public string? BussinesName { get; set; } = string.Empty;

        public string? Photo { get; set; } = string.Empty;

        public string? NomalizedUserName { get; set; } = string.Empty;

        public bool? EmailConfirmed { get; set; } = false;

        public string? Password { get; set; } = string.Empty;

        public bool? PhoneConfirmed { get; set; } = false;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsDeleted { get; set; } = false;

        public DateTime? DeletedDate { get; set; }

        public Guid? RegionId { get; set; }
        public Guid? SubAreaId { get; set; }
        public string? RegionName { get; set; }
        public string? SubAreaName { get; set; }
    }
}
