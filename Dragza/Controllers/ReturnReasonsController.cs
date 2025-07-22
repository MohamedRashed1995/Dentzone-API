// ReturnReasonsController.cs
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dragza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReturnReasonsController : ControllerBase
{
    private readonly IReturnReasonService _service;

    public ReturnReasonsController(IReturnReasonService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReturnReasonDto>>> GetAll()
    {
        var reasons = await _service.GetAllAsync();
        return Ok(reasons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReturnReasonDto>> GetById(Guid id)
    {
        try
        {
            var reason = await _service.GetByIdAsync(id);
            return Ok(reason);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ReturnReasonDto>> Create([FromBody] CreateReturnReasonDto createDto)
    {
        try
        {
            var created = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReturnReasonDto updateDto)
    {
        try
        {
            await _service.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}