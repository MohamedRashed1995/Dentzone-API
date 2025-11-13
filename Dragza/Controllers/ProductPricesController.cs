using ClosedXML.Excel;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dragza.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductPricesController : ControllerBase
    {
        private readonly IProductPriceService _priceService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductPricesController(
            IProductPriceService priceService,
            IHttpContextAccessor httpContextAccessor)
        {
            _priceService = priceService;
            _httpContextAccessor = httpContextAccessor;
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetPrice(Guid id)
        //{
        //    Implementation to get single price
        //}

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user identity");

            return userId;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Inventory")]
        public async Task<IActionResult> CreatePrice([FromBody] CreateProductPriceDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _priceService.CreateProductPriceAsync(dto, userId);
            return Ok( result);
        }

        [HttpGet("my-prices/{userId}")]
        public async Task<IActionResult> GetUserPrices(Guid userId)
        {
            //var userId = GetCurrentUserId();
            var prices = await _priceService.GetPricesByUserAsync(userId);
            return Ok(prices);
        }

        [HttpGet("best-prices")]
        public async Task<IActionResult> GetProductsBestPrices(int page = 1, int size = 10)
        {
            var bestPrices = await _priceService.GetProductsBestPricesAsync( page, size );
            return Ok(bestPrices);
        }

        [HttpGet("best-prices-bysorting")]
        public async Task<IActionResult> GetProductsBestPricesbysorting(int sort, int page = 1, int size = 10)
        {
            var bestPrices = await _priceService.GetProductsBestPricesSortingAsync(sort,page,size);
            return Ok(bestPrices);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetPricesByCategory(Guid categoryId)
        {
            try
            {
                var prices = await _priceService.GetPricesByCategoryAsync(categoryId);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving prices");
            }
        }


        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetPricesByProduct(Guid productId)
        {
            try
            {
                var prices = await _priceService.GetPricesByProductAsync(productId);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving prices");
            }
        }


        [HttpGet("by-inventory-user/{userId}")]
        public async Task<IActionResult> GetPricesByInventoryUser(Guid userId)
        {
            try
            {
                var prices = await _priceService.GetPricesByInventoryUserAsync(userId);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving prices");
            }
        }
        [HttpGet("by-inventory-product")]
        public async Task<IActionResult> GetPricesByInventoryProduct(Guid userId , Guid productId)
        {
            try
            {
                var prices = await _priceService.GetPricesByInventoryUserAsync(userId);
                var result = prices.Where(p => p.ProductId == productId).FirstOrDefault();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving prices");
            }
        }


        //[HttpPost("upload-products")]
        //public async Task<IActionResult> UploadProducts(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("Please upload a valid Excel file.");

        //    // قراءة الملف في الذاكرة
        //    using var stream = new MemoryStream();
        //    await file.CopyToAsync(stream);
        //    stream.Position = 0;

        //    using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
        //    var worksheet = workbook.Worksheet(1);

        //    // نحصل على عدد الصفوف
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // تخطي العنوان

        //    foreach (var row in rows)
        //    {
        //        string productName = row.Cell(1).GetString();

        //        // افتراضياً نتحقق من وجود المنتج في قاعدة البيانات
        //        bool existsInDb = await _priceService.GetAllProductPricesAsync(
        //            .AnyAsync(p => p.Name == productName);

        //        if (!existsInDb)
        //        {
        //            // كتابة الخطأ في العمود السادس مثلاً
        //            row.Cell(6).Value = $"Product '{productName}' not found in Products table";
        //            row.Cell(6).Style.Fill.BackgroundColor = XLColor.LightPink;
        //        }
        //        else
        //        {
        //            row.Cell(6).Value = "OK";
        //        }
        //    }

        //    // حفظ الملف المعدل في stream جديد
        //    using var outputStream = new MemoryStream();
        //    workbook.SaveAs(outputStream);
        //    outputStream.Position = 0;

        //    // إرجاع الملف للتحميل
        //    return File(outputStream.ToArray(),
        //        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        "ProductsResult.xlsx");
        //}
    }
}
