using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _uploadPath;
        private readonly string[] _allowedExtensions;
        private readonly FileSettings _fileSettings;

        public LocalFileStorageService(IOptions<FileSettings> fileSettings)
        {
            _fileSettings = fileSettings.Value;
            Directory.CreateDirectory(_fileSettings.UploadPath);
        }


        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded");

            if (file.Length > _fileSettings.MaxFileSize)
                throw new InvalidOperationException(
                    $"File exceeds maximum size of {_fileSettings.MaxFileSize} bytes");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_fileSettings.AllowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    $"Invalid file type. Allowed: {string.Join(", ", _fileSettings.AllowedExtensions)}");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_fileSettings.UploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        public async Task<byte[]> GetFileAsync(string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);
            return await System.IO.File.ReadAllBytesAsync(filePath);
        }
    }
}
