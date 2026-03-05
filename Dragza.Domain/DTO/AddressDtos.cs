using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CreateAddressDto
    {
        public Guid UserId { get; set; }
        public string AddressLine { get; set; }
    }

    public class UpdateAddressDto
    {
        public string AddressLine { get; set; }
    }

    public class AddressResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string AddressLine { get; set; }="";
    }
}
