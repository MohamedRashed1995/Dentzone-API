using Dragza.Application.Interface;
using Dragza.Domain.DTO.ReturnOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dragza.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/returns")]
    public class ReturnOrdersController : ControllerBase
    {
        private readonly IReturnOrderService _returnService;
        private readonly IHttpContextAccessor _contextAccessor;

        public ReturnOrdersController(IReturnOrderService returnService, IHttpContextAccessor contextAccessor)
        {
            _returnService = returnService;
            _contextAccessor = contextAccessor;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReturn([FromBody] CreateReturnOrderDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _returnService.CreateReturnAsync(dto, userId);
            return CreatedAtAction(nameof(GetReturn), new { id = result.Id }, result);
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateReturnStatusDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _returnService.UpdateReturnStatusAsync(id, dto, userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReturn(Guid id)
        {
            var result = await _returnService.GetReturnAsync(id);
            return Ok(result);
        }

        [HttpGet("pharmacy")]
        [Authorize]
        public async Task<IActionResult> GetPharmacyReturns()
        {
            var userId = GetCurrentUserId();
            var results = await _returnService.GetPharmacyReturnsAsync(userId);
            return Ok(results);
        }

        [HttpGet("vendor")]
        [Authorize]
        public async Task<IActionResult> GetVendorReturns()
        {
            var userId = GetCurrentUserId();
            var results = await _returnService.GetVendorReturnsAsync(userId);
            return Ok(results);
        }

        [HttpGet("reasons")]
        public async Task<IActionResult> GetReturnReasons()
        {
            var results = await _returnService.GetReturnReasonsAsync();
            return Ok(results);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user identity");
            return userId;
        }
    }
}
