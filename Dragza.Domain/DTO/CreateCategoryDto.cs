using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Pref { get; set; }
        public string Description { get; set; }
        public Guid? MainCategoryId { get; set; }
        public string? ArabicName { get; set; }


    }
}
