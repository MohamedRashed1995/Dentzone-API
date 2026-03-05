using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class InventoryPopularDto
    {
        public Guid InventoryId { get; set; }
        public string InventoryName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<AddressResponseDto> Addresses { get; set; } = new();
        public List<PopularProductDto> Products { get; set; } = new();
    }
}
