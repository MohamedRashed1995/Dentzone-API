using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class HomeDto
    {
        public List<BannerDto> Banners { get; set; } = new();
        public List<CategoryAppDto> Categories { get; set; } = new();
        public List<ProductAppDto> Products { get; set; } = new(); // بدل Inventories
    }
}
