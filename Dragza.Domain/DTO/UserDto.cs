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
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessName { get; set; }
        public bool IsPharmacy { get; set; }
        public string Region { get; set; }
        public decimal MinOrder { get; set; }
        public bool IsActive { get; set; }
        public Guid Accountid { get; set; }

        public string? BussinesName { get; set; }

        public string? Photo { get; set; }

        public string? NomalizedUserName { get; set; }

        public bool? EmailConfirmed { get; set; }

        public string? Password { get; set; }

        public bool? PhoneConfirmed { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid? RegionId { get; set; }
    }
}
