// MainCategoriesController.cs
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dragza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MainCategoriesController : ControllerBase
{
    private readonly IMainCategoryService _service;

    public MainCategoriesController(IMainCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MainCategoryDto>>> GetAll(int lang)
    {
        var mainCategories = await _service.GetAllAsync();
        if (lang == 0) // Assuming 0 is for Arabic
        {
            foreach (var category in mainCategories)
            {
                category.Name = category.ArabicName ; // Simulating language change for demonstration
            }
        }
        return Ok(mainCategories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MainCategoryWithRelationsDto>> GetById(Guid id, int lang)
    {
        var mainCategory = await _service.GetByIdAsync(id);
        if(lang == 0)
        {
            mainCategory.Name = mainCategory.ArabicName ; // Simulating language change for demonstration

        }
        else
        {
            mainCategory.Name = mainCategory.Name; // Simulating language change for demonstration
            mainCategory.ArabicName = mainCategory.ArabicName; // Simulating language change for demonstration
        }
        return Ok(mainCategory);
    }

    [HttpPost]
    public async Task<ActionResult<MainCategoryDto>> Create([FromBody] CreateMainCategoryDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMainCategoryDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{mainCategoryId}/categories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(Guid mainCategoryId , int lang)
    {
        var categories = await _service.GetCategoriesAsync(mainCategoryId);
        if (lang == 0) // Assuming 0 is for Arabic
        {
            foreach (var category in categories)
            {
                category.Name = category.ArabicName ; // Simulating language change for demonstration
            }
        }
        else
        {
            foreach (var category in categories)
            {
                category.Name = category.Name; // Simulating language change for demonstration
                category.ArabicName = category.ArabicName; // Simulating language change for demonstration
            }
        }
        return Ok(categories);
    }

    [HttpGet("{mainCategoryId}/products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(Guid mainCategoryId, int lang)
    {
        var products = await _service.GetProductsAsync(mainCategoryId);
        if (lang == 0) // Assuming 0 is for Arabic
        {
            foreach (var product in products)
            {
                product.Name = product.ArabicName ; // Simulating language change for demonstration
            }
        }
        else
        {
            foreach (var product in products)
            {
                product.Name = product.Name; 
                product.ArabicName = product.ArabicName; 
            }
        }
        return Ok(products);
    }
}