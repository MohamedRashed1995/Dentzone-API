using ClosedXML.Excel;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Security.Claims;

namespace Dragza.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductPricesController : ControllerBase
    {
        private readonly IProductPriceService _priceService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProductPricesController> _logger;
        private readonly IMemoryCache _memoryCache;

        public ProductPricesController(
            IProductPriceService priceService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ProductPricesController> logger,
            IMemoryCache memoryCache)
        {
            _priceService = priceService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _memoryCache = memoryCache;
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
            var stopWatch = Stopwatch.StartNew();
            string cacheKey = $"productsWithPrice__{sort}_{page}_{size}";
            if (!_memoryCache.TryGetValue(cacheKey, out List<ProductBestPriceDto> cachedProductPrice))
            {
                var bestPrices = await _priceService.GetProductsBestPricesSortingAsync(sort, page, size);
                _logger.LogInformation("Cache is empty");

                cachedProductPrice= bestPrices.ToList();
                var cacheOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromMinutes(2))   // يتجدد مع الاستخدام
                   //.SetAbsoluteExpiration(TimeSpan.FromMinutes(30)) // أقصى مدة
                   .SetPriority(CacheItemPriority.High);

                _memoryCache.Set(cacheKey, cachedProductPrice, cacheOptions);
                _logger.LogInformation("Data cached successfully");
            }
            else
            {
                   _logger.LogInformation("Data retrieved from cache");
            }

                stopWatch.Stop();
            _logger.LogInformation("GetAllProducts executed in {ElapsedMilliseconds} ms", stopWatch.ElapsedMilliseconds);
            return Ok(cachedProductPrice);
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
