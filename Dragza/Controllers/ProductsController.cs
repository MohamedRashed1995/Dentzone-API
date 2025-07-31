using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dragza.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductPriceService _productPriceService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(IProductService productService, IHttpContextAccessor httpContextAccessor, IProductPriceService productPriceService)
        {
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
            _productPriceService = productPriceService;
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
        public async Task<IActionResult> GetAllProducts([FromQuery] int lang,[FromQuery] bool includeDeleted = false, [FromQuery] string search = null)
        {
			var products = await _productService.GetAllProductsAsync(includeDeleted, search);
			if (lang == 0) // Assuming 0 is for Arabic
			{
			    foreach (var product in products)
			    {
			        product.Name = product.ArabicName ; // Simulating language change for demonstration
			    }
			}
			return Ok(products);

			
		}

		[HttpGet("product-prices-with-data")]
		public async Task<IActionResult> GetAllProductPricesWithProductData([FromQuery] int lang, [FromQuery] bool includeDeleted = false, [FromQuery] string search = null)
		{

			var productPrices = await _productPriceService.GetAllProductPricesAsync(includeDeleted, search);
			if (lang == 0) // Assuming 0 is for Arabic
			{
				foreach (var product in productPrices)
				{
					product.ProductName = product.ProductArabicName; // Simulating language change for demonstration
				}
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
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
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

            var products = await _productService.GetPricesWithAllProductByInventoryId(userId);

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Products");

            // Header
            worksheet.Cell(1, 1).Value = "ProductPriceId";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "SalesPrice";
            worksheet.Cell(1, 4).Value = "PurchasePrice";
            worksheet.Cell(1, 5).Value = "StockQuantity";
            worksheet.Cell(1, 6).Value = "ProductId";

            int row = 2;
            foreach (var product in products)
            {
                worksheet.Cell(row, 1).Value = product.Id.ToString();
                worksheet.Cell(row, 2).Value = product.Product.Name;
                worksheet.Cell(row, 3).Value = product.SalesPrice;
                worksheet.Cell(row, 4).Value = product.PurchasePrice;
                worksheet.Cell(row, 5).Value = product.StockQuantity; // Placeholder for quantity
                worksheet.Cell(row, 6).Value = product.ProductId.ToString(); // Placeholder for quantity
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

                await _productPriceService.UpdateProductPriceAndQuantityAsync(productId,productPriceId, salesPrice, purchasePrice, quantity,userId);
            }

            return Ok("Products updated successfully.");
        }
    }
}
