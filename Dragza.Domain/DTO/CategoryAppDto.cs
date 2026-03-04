using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CategoryAppDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ArabicName { get; set; }
        public string? ImageFile { get; set; }
    }
}
