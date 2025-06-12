using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task<byte[]> GetFileAsync(string fileName);
    }
}
