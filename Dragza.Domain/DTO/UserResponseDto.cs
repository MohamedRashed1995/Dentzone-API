using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public string? Address { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
