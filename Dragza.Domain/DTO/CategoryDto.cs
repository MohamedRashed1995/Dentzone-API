using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Pref { get; set; }
        public string Description { get; set; }
        public Guid MainCategoryId { get; set; }
        public string MainCategory { get; set; }
        public string? ArabicName { get; set; }

    }
}
