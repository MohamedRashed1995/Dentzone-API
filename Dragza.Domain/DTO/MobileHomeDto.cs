using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class MobileHomeDto
    {
        public List<BannerDto> Banners { get; set; } = new();
        public List<CategoryAppDto> Categories { get; set; } = new();
        public List<InventorySectionDto> Inventories { get; set; } = new();
    }
}
