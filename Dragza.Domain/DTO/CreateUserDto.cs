using Dragza.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CreateUserDto
    {
        
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        //public string AddressLines { get; set; } = string.Empty;
        public List<string> AddressLines { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public Guid? RoleId { get; set; }
        
    }

}
