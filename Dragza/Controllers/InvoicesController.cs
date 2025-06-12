// InvoicesController.cs
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dragza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InvoicesController(IInvoiceService invoiceService, IHttpContextAccessor httpContextAccessor)
    {
        _invoiceService = invoiceService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvoice(Guid id)
    {
        try
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            return Ok(invoice);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetInvoiceByOrder(Guid orderId)
    {
        try
        {
            var invoice = await _invoiceService.GetInvoiceByOrderIdAsync(orderId);
            return Ok(invoice);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserInvoices(Guid userId)
    {
        // Verify current user has access to these invoices
        var currentUserId = GetCurrentUserId();

        var invoices = await _invoiceService.GetUserInvoicesAsync(userId);
        return Ok(invoices);
    }

    [HttpGet("Inventory/{userId}")]
    public async Task<IActionResult> GetInventoryInvoices(Guid userId)
    {
        // Verify current user has access to these invoices
        var currentUserId = GetCurrentUserId();

        var invoices = await _invoiceService.GetInventoryInvoicesAsync(userId);
        return Ok(invoices);
    }

    [HttpGet("date-range")]
    [Authorize]
    public async Task<IActionResult> GetInvoicesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest("Start date must be before end date");
        }

        var invoices = await _invoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);
        return Ok(invoices);
    }

    [HttpPost("generate/{orderId}")]
    [Authorize]
    public async Task<IActionResult> GenerateInvoice(Guid orderId)
    {
        try
        {
            var invoice = await _invoiceService.GenerateInvoiceForOrderAsync(orderId);
            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity");

        return userId;
    }
}