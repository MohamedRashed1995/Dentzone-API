// WebAPI/Controllers/CouponsController.cs
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dragza.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CouponsController(ICouponService couponService, IHttpContextAccessor httpContextAccessor)
        {
            _couponService = couponService;
            _httpContextAccessor = httpContextAccessor;
        }

        //[HttpPost("apply")]
        //public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto dto, [FromQuery] Guid orderId)
        //{
        //    try
        //    {
        //        var userId = GetCurrentUserId();
        //        var result = await _couponService.ApplyCouponAsync(dto.Code, orderId, userId);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new CouponApplicationResultDto
        //        {
        //            Success = false,
        //            Message = ex.Message
        //        });
        //    }
        //}

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCoupons([FromQuery] decimal? orderTotal)
        {
            try
            {
                var userId = GetCurrentUserId();
                var coupons = await _couponService.GetAvailableCouponsAsync(userId, orderTotal ?? 0);
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
        {
            try
            {
                var result = await _couponService.CreateCouponAsync(dto);
                return CreatedAtAction(nameof(GetCoupon), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous] // Optional: Make public if needed
        public async Task<IActionResult> GetAllCoupons([FromQuery] bool activeOnly = false)
        {
            try
            {
                var coupons = await _couponService.GetAllCouponsAsync(activeOnly);
                return Ok(coupons);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoupon(Guid id)
        {
            try
            {
                var coupon = await _couponService.GetCouponByIdAsync(id);
                return Ok(coupon);
            }
           
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(Guid id, [FromBody] UpdateCouponDto dto)
        {
            try
            {
                var result = await _couponService.UpdateCouponAsync(id, dto);
                return Ok(result);
            }
           
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(Guid id)
        {
            try
            {
                await _couponService.DeleteCouponAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/usages")]
        public async Task<IActionResult> GetCouponUsages(Guid id)
        {
            try
            {
                var usages = await _couponService.GetCouponUsagesAsync(id);
                return Ok(usages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/redemptions")]
        public async Task<IActionResult> GetRedemptionCount(Guid id)
        {
            try
            {
                var count = await _couponService.GetRedemptionCountAsync(id);
                return Ok(new { couponId = id, redemptionCount = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _couponService.ApplyCouponAsync(dto.Code, dto.amount, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
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
}