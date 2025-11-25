using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class ProductAddDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;

        public string? Preef { get; set; }

        public string? Description { get; set; }

        public Guid CategoryId { get; set; }
        public Guid? ActiveIngerdientId { get; set; }
        public string? ArabicName { get; set; }
    }
}
