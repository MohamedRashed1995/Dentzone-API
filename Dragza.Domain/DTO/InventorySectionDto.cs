using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class InventorySectionDto
    {
        public Guid InventoryId { get; set; }
        public string InventoryName { get; set; } = null!;
        public List<ProductAppDto> Products { get; set; } = new();
    }
}
