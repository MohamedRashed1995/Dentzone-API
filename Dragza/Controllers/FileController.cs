using Dragza.Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dragza.API.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileStorageService _fileStorage;

        public FileController(IFileStorageService fileStorage)
        {
            _fileStorage = fileStorage;
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            try
            {
                var fileBytes = await _fileStorage.GetFileAsync(fileName);
                var contentType = GetContentType(fileName);
                return File(fileBytes, contentType);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }
}
