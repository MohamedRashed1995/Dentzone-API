// GovernatesController.cs
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Dragza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubAreasController : ControllerBase
{
    private readonly IGovernateService _service;

    public SubAreasController(IGovernateService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var governates = await _service.GetAllAsync(false);
        return Ok(governates);
    }

    [HttpGet("GetAllActive")]
    public async Task<ActionResult> GetAllActive()
    {
        var governates = await _service.GetAllAsync(true);
        return Ok(governates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var governate = await _service.GetByIdAsync(id);
        return Ok(governate);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateGovernateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGovernateDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPut("{id}/ChangeStatus")]
    public async Task<IActionResult> ChangeStatus(Guid id)
    {
        await _service.ChangeStatus(id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }


    [HttpGet("{regionId}/Region")]
    public async Task<ActionResult> GetByRegionId(Guid regionId)
    {
        var governate = await _service.GetAllWithRegionId(regionId);
        return Ok(governate);
    }
}