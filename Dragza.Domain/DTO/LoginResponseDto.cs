using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class LoginResponseDto
    {
        // User info زي Register
        public Guid Id { get; set; }
        //public string FirstName { get; set; } = string.Empty;
        //public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        //public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public List<AddressResponseDto> Addresses { get; set; } = new();
        // JWT info
        public string Token { get; set; } = string.Empty;
        public bool HasDetails { get; set; }
        public string Role { get; set; } = string.Empty;
    }

}
