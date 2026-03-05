using Dragza.Domain.Models;
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

        //public string FirstName { get; set; } = string.Empty;
        //public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public List<AddressResponseDto> Addresses { get; set; } = new();
        //public ICollection<Address> Addresses { get; set; } = new List<Address>();
        //public List<string> Role { get; set; } = new List<string>();
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();

    }
}
