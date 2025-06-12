using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dragza.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] bool includeDeleted = false)
        {
            var products = await _productService.GetAllProductsAsync(includeDeleted);
            return Ok(products);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
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
        public async Task<IActionResult> GetBestSellingProducts( [FromQuery] int top = 10)
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
    }
}
