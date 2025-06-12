using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class MainCategoryWithRelationsDto : MainCategoryDto
    {
        public IEnumerable<CategoryDto> Categories { get; set; }
        public IEnumerable<ProductDto> Products { get; set; }
    }
}
