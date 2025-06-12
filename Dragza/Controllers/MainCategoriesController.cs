// MainCategoriesController.cs
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
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
    public async Task<ActionResult<IEnumerable<MainCategoryDto>>> GetAll()
    {
        var mainCategories = await _service.GetAllAsync();
        return Ok(mainCategories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MainCategoryWithRelationsDto>> GetById(Guid id)
    {
        var mainCategory = await _service.GetByIdAsync(id);
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

    //[HttpGet("{mainCategoryId}/categories")]
    //public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(Guid mainCategoryId)
    //{
    //    var categories = await _service.GetCategoriesAsync(mainCategoryId);
    //    return Ok(categories);
    //}

    //[HttpGet("{mainCategoryId}/products")]
    //public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(Guid mainCategoryId)
    //{
    //    var products = await _service.GetProductsAsync(mainCategoryId);
    //    return Ok(products);
    //}
}