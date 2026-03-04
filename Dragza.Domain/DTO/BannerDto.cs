using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class BannerDto
    {
        public Guid Id { get; set; }
        public string? ImageName { get; set; } = null!;
        public int? Order { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
