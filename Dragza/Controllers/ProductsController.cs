using ClosedXML.Excel;
using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Dragza.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductPriceService _productPriceService;
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DragzaContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly IMemoryCache _memoryCache;

        public ProductsController(IProductService productService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IProductPriceService productPriceService, ICategoryService categoryService, DragzaContext context, ILogger<ProductsController> logger, IMemoryCache memoryCache)
        {
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
            _productPriceService = productPriceService;
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
            _context = context;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user identity");

            return userId;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id , int lang)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if(lang == 0)
            {
                product.Name = product.ArabicName; // Simulating language change for demonstration
            }
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int lang,[FromQuery] bool includeDeleted = false, [FromQuery] string search = null, int page = 1, int size = 10)
        {
			var products = await _productService.GetAllProductsAsync(includeDeleted, search,page,size);
			if (lang == 0) // Assuming 0 is for Arabic
			{
			    foreach (var product in products)
			    {
			        product.Name = product.ArabicName ; // Simulating language change for demonstration
			    }
			}
			return Ok(products);

			
		}
        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetAllProductss(
    [FromQuery] int lang,
    [FromQuery] bool includeDeleted = false,
    [FromQuery] string search = null,
    int page = 1,
    int size = 10)
        {

            var PDS = await _unitOfWork.ProductRepository.GetAllAsync();

            var s= PDS.Where(a => a.ProductCode == null).ToList();

            foreach (var prop in s)
            {
                prop.ProductCode = GenerateProductCode();
                _unitOfWork.ProductRepository.Update(prop);
            
            }
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
               
                var inner = ex.InnerException?.Message;
                throw new Exception(inner);
            }

            var stopWatch = Stopwatch.StartNew();
            string cacheKey = $"products_{lang}_{includeDeleted}_{search}_{page}_{size}";

            if (!_memoryCache.TryGetValue(cacheKey, out object cachedResult))
            {
                var products = await _productService.GetAllProductsAsync(includeDeleted, search, page, size);
                _logger.LogInformation("Cache is empty");
                int totalCount = _context.Products.Count();
                int totalPages = (int)Math.Ceiling((double)totalCount / size);

                if (lang == 0) // Arabic
                {
                    foreach (var product in products)
                    {
                        product.Name = product.Name;
                        product.ArabicName = product.ArabicName;
                    }
                }

                cachedResult = new
                {
                    success = true,
                    data = products,
                    totalPages = totalPages,
                    totalItems = totalCount
                };

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1))   // يتجدد مع الاستخدام
                    //.SetAbsoluteExpiration(TimeSpan.FromMinutes(30)) // أقصى مدة
                    .SetPriority(CacheItemPriority.High);

                _memoryCache.Set(cacheKey, cachedResult, cacheOptions);
                _logger.LogInformation("Data cached successfully");
            }
            else {
                _logger.LogInformation("Data retrieved from cache");

            }
            stopWatch.Stop();
            _logger.LogInformation("GetAllProducts executed in {ElapsedMilliseconds} ms", stopWatch.ElapsedMilliseconds);
            return Ok(cachedResult);
        }
        [HttpGet("AllProducts")]
        public async Task<IActionResult> GetAllProduct(string search)
        {
            var products = await _productService.GetProductsAsync(search);
          
            return Ok(products);


        }


        [HttpGet("product-prices-with-data")]
		public async Task<IActionResult> GetAllProductPricesWithProductData([FromQuery] int lang, [FromQuery] bool includeDeleted = false, [FromQuery] string search = null, int page = 1, int size = 10)
		{

			var productPrices = await _productPriceService.GetAllProductPricesAsync(includeDeleted, search,page,size);
			if (lang == 0) // Assuming 0 is for Arabic
			{
				foreach (var product in productPrices)
				{
					product.ProductName = product.ProductArabicName; // Simulating language change for demonstration
				}
			}
			return Ok(productPrices);
		}



        [HttpGet("product-prices-with-data/{productId}")]
        public async Task<IActionResult> GetAllProductPricesWithProductDatabyId(Guid productId)
        {

            var productPrices = await _productPriceService.GetAllProductAsync(productId);

            
                foreach (var product in productPrices)
                {
                    product.ProductName = product.ProductName;
                    product.ProductArabicName = product.ProductArabicName;
            }
            
            return Ok(productPrices);
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] UpdateProductDto dto)
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteProduct(Guid id)
        {
            await _productService.SoftDeleteProductAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreProduct(Guid id)
        {
            await _productService.RestoreProductAsync(id);
            return NoContent();
        }

        [HttpGet("by-ingredient/{activeIngredientId}")]
        public async Task<IActionResult> GetProductsByActiveIngredient(Guid activeIngredientId)
        {
            try
            {
                var products = await _productService.GetProductsByActiveIngredientAsync(activeIngredientId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellingProducts([FromQuery] int top = 10)
        {
            try
            {
                var result = await _productService.GetBestSellingProductsAsync(top);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategoryId(Guid categoryId)
        {
            //var products = await _productService.GetProductsByCategoryAsync(categoryId);
            var products = await _productPriceService.GetProductByCategoryAsync(categoryId);
            if (!products.Any())
            {
                return NotFound("No products found for this category.");
            }

            return Ok(products);
        }

        [HttpGet("export-excel")]
        [Authorize(Roles = "Inventory")]
        public async Task<IActionResult> ExportProductsToExcel()
        {
            var userId = GetCurrentUserId();

            //  var products = await _productService.GetPricesWithAllProductByInventoryId(userId);
           var products=  await _productPriceService.GetPricesByUserAsync(userId);

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Products");

            // Header
            worksheet.Cell(1, 1).Value = "ProductPriceId";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "SalesPrice";
            worksheet.Cell(1, 4).Value = "PurchasePrice";
            worksheet.Cell(1, 5).Value = "StockQuantity";
         
            worksheet.Cell(1, 6).Value = "Discount";
            worksheet.Cell(1, 7).Value = "MaxQuantity";
         

            int row = 2;
            foreach (var product in products)
            {
                worksheet.Cell(row, 1).Value = product.Id.ToString();
                worksheet.Cell(row, 2).Value = product.ProductArabicName;
                worksheet.Cell(row, 3).Value = product.SalesPrice;
                worksheet.Cell(row, 4).Value = product.PurchasePrice;
                worksheet.Cell(row, 5).Value = product.StockQuantity; // Placeholder for quantity
                double discount = (double)((product.SalesPrice - product.PurchasePrice) / product.SalesPrice) * 100;
                var result = double.IsFinite(discount) ? discount.ToString("F2") : "0.00";
                worksheet.Cell(row, 6).Value = result; // Placeholder for quantity
                worksheet.Cell(row, 7).Value = product.MaxQuantity; // Placeholder for quantity
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
        }


        [HttpPost("import-excel")]
        [Authorize(Roles = "Inventory")]
        public async Task<IActionResult> ImportProductsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            var userId = GetCurrentUserId();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header
            {
                var productPriceId = Guid.Parse(row.Cell(1).GetString());
                var salesPrice = decimal.Parse(row.Cell(3).GetString());
                var purchasePrice = int.Parse(row.Cell(4).GetString());
                var quantity = int.Parse(row.Cell(5).GetString());
                var productId = Guid.Parse(row.Cell(6).GetString());
                //var productName = row.Cell(1).GetString().Trim();
                //if (string.IsNullOrWhiteSpace(productName))
                //    continue;

                //var salesPrice = row.Cell(2).GetValue<decimal>();
                //var discountRate = row.Cell(3).GetValue<decimal>();
                //var stockQuantity = row.Cell(4).GetValue<int>();
                //var maxQuantity = row.Cell(5).GetValue<int>();

              //  await _productPriceService.UpdateProductPriceAndQuantityAsync(productId,productPriceId, salesPrice, purchasePrice, quantity, quantity,userId);
            }

            return Ok("Products updated successfully.");
        }


        //[HttpPost("ImportAddProductsFromExcel")]
       
        //public async Task<IActionResult> ImportAddProductsFromExcel(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");
        //    var userId = GetCurrentUserId();

        //    using var stream = new MemoryStream();
        //    await file.CopyToAsync(stream);
        //    using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
        //    var worksheet = workbook.Worksheet(1);
         

            

        //    foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header
        //    {
              

        //        var productName = row.Cell(1).GetString().Trim();
        //        if (string.IsNullOrWhiteSpace(productName))
        //            continue;
        //        var product = await _productService.GetproductbyName(productName);
        //        var productPrice = product.ProductPrices.Where(a => a.InventoryUserId == userId).FirstOrDefault();

        //        var salesPrice = row.Cell(2).GetValue<decimal>();
        //        var discountRate = row.Cell(3).GetValue<decimal>();
        //        var stockQuantity = row.Cell(4).GetValue<int>();
        //        var maxQuantity = row.Cell(5).GetValue<int>();
        //        var purchasePrice = row.Cell(6).GetValue<int>();

        //        await _productPriceService.UpdateProductPriceAndQuantityAsync(product.Id, productPrice.Id, salesPrice, purchasePrice, stockQuantity, maxQuantity, userId);
        //    }

        //    return Ok("Products updated successfully.");
        //}


        [HttpPost("ImportAddProductsFromExcel")]
        public async Task<IActionResult> ImportAddProductsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var userId = GetCurrentUserId();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            // إضافة عمود خطأ
            var errorColumn = worksheet.Column(6);
            worksheet.Cell(1, 7).Value = "Error";
            worksheet.Cell(1, 7).Style.Fill.BackgroundColor = XLColor.Red;
            worksheet.Cell(1, 7).Style.Font.FontColor = XLColor.White;

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                string errorMessage = "";

                var productName = row.Cell(1).GetString().Trim();
                var ProductCode = row.Cell(6).GetValue<int>();
                if (string.IsNullOrWhiteSpace(productName))
                    continue;

                var product = await _productService.GetproductbyCode(ProductCode);

                if (product == null)
                {
                    errorMessage = $"Product '{productName}' not found in Products table";
                    row.Cell(7).Value = errorMessage;
                    row.Cell(7).Style.Fill.BackgroundColor = XLColor.LightPink;
                  
                }
                else
                {
                    errorMessage = $"Product '{productName}' Is ALready Exist";
                    row.Cell(7).Value = errorMessage;
                    row.Cell(7).Style.Fill.BackgroundColor = XLColor.LightPink;
                 
                }

                    var productPrice = product.ProductPrices
                        .FirstOrDefault(a => a.InventoryUserId == userId);

                if (productPrice == null)
                {
                    errorMessage = $"Price row for '{productName}' not found";
                    row.Cell(7).Value = errorMessage;
                    row.Cell(7).Style.Fill.BackgroundColor = XLColor.LightPink;
                  
                }
                else
                {
                    errorMessage = $"Price row for '{productName}' is Already Exist";
                    row.Cell(7).Value = errorMessage;
                    row.Cell(7).Style.Fill.BackgroundColor = XLColor.LightPink;
                  
                }

                    try
                    {
                        var salesPrice = row.Cell(2).GetValue<decimal>();
                        var discountRate = row.Cell(3).GetValue<decimal>();
                        var stockQuantity = row.Cell(4).GetValue<int>();
                        var maxQuantity = row.Cell(5).GetValue<int>();
                        //var ProductCode = row.Cell(6).GetValue<int>();
                    // var purchasePrice = row.Cell(6).GetValue<int>();
                    var purchasePrice = salesPrice - (salesPrice * (decimal)(discountRate / 100));

                    await _productPriceService.UpdateProductPriceAndQuantityAsync(
                            product.Id,
                           productPrice?.Id,
                            salesPrice,
                            purchasePrice,
                            stockQuantity,
                            maxQuantity,
                            userId,
                            discountRate
                        );
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    row.Cell(7).Value = errorMessage;
                    row.Cell(7).Style.Fill.BackgroundColor = XLColor.LightPink;
                }
            }

            // إرجاع الملف بعد التعديل
            using var outputStream = new MemoryStream();
            workbook.SaveAs(outputStream);
            outputStream.Position = 0;

            return File(
                outputStream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ProcessedProducts.xlsx"
            );
        }
        public static int GenerateProductCode()
        {
            return RandomNumberGenerator
                .GetInt32(10000000, 99999999);

        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
           

            //  var userId = GetCurrentUserId();
            List<ProductAddDto> productDtos = new List<ProductAddDto>();

          

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var errorColumn = worksheet.Column(6);
            worksheet.Cell(1, 7).Value = "Error";
            worksheet.Cell(1, 7).Style.Fill.BackgroundColor = XLColor.Red;
            worksheet.Cell(1, 7).Style.Font.FontColor = XLColor.White;


            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                string errorMessage = "";

                var productNameEn = row.Cell(1).GetString().Trim();
                var productNameAr = row.Cell(2).GetString().Trim();
                if (string.IsNullOrWhiteSpace(productNameEn))
                    continue;

                var product = await _productService.GetproductbyName(productNameEn);

                if (product != null)
                {
                    errorMessage = $"Product '{productNameAr}' Is ALready Exist";
                    row.Cell(7).Value = errorMessage;
                    row.Cell(7).Style.Fill.BackgroundColor = XLColor.LightPink;
                    continue;
                }
               

             
                try
                {
                    var preef = row.Cell(3).GetString().Trim();
                    var description = row.Cell(4).GetString().Trim();
                    var ActiveName = row.Cell(5).GetString().Trim();
                    var categoryName = row.Cell(6).GetString().Trim();
                    var category = await _categoryService.GetCategoryByNameAsync(categoryName);
                    var active = await _unitOfWork.ActiveIngredientRepository.GetByName(ActiveName);


                    productDtos.Add(new ProductAddDto
                    {
                        Id=Guid.NewGuid(),
                        ArabicName = productNameAr,
                        Name = productNameEn,
                        Description = description,
                        Preef = preef,
                        CategoryId = category.Id,
                        ActiveIngerdientId = active.Id,

                    });

                    foreach (var item in productDtos)
                    {
                        _productService.AddProduct(item);
                    }

                   


                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    row.Cell(7).Value = errorMessage;
                    row.Cell(7).Style.Fill.BackgroundColor = XLColor.LightPink;
                }
            }

            // إرجاع الملف بعد التعديل
            using var outputStream = new MemoryStream();
            workbook.SaveAs(outputStream);
            outputStream.Position = 0;

            return File(
                outputStream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ProcessedProducts.xlsx"
            );
        }


    }
}
