using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class FileSettings
    {
        public string UploadPath { get; set; } = "Uploads";
        public long MaxFileSize { get; set; } = 5 * 1024 * 1024; // 5MB
        public string AllowedExtensions { get; set; } = ".pdf,.jpg,.jpeg,.png";
    }
}
