    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

namespace Dragza.Domain.Models
{
    public class Banner
    {
        public Guid Id { get; set; }
        public string? ImageName { get; set; }
        public int? Order { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
