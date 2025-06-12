// RegionsController.cs
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Dragza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegionsController : ControllerBase
{
    private readonly IRegionService _service;

    public RegionsController(IRegionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegionDto>>> GetAll()
    {
        var regions = await _service.GetAllAsync();
        return Ok(regions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RegionWithUsersDto>> GetById(Guid id)
    {
        var region = await _service.GetByIdAsync(id);
        return Ok(region);
    }

    [HttpPost]
    public async Task<ActionResult<RegionDto>> Create([FromBody] CreateRegionDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRegionDto dto)
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

    [HttpGet("{regionId}/users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(Guid regionId)
    {
        var users = await _service.GetUsersByRegionAsync(regionId);
        return Ok(users);
    }
}