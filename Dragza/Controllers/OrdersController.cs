using Dragza.Application.Interface;
using Dragza.Domain.DTO.Order;
using Dragza.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dragza.API.Controllers
{
   // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IBalanceService _balanceService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersController(IOrderService orderService, IHttpContextAccessor httpContextAccessor, IBalanceService balanceService)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
            _balanceService = balanceService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder(string userIID)
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.CreateOrderByIdAsync(userIID);
            if (!order.Success)
            {
                return BadRequest(new { message = order.Message });
            }
            else
            {
                return Ok(new
                {
                    message = order.Message

                });
            }
         
        }

        [HttpPut("approve/{orderId}")]
        [Authorize]
        public async Task<IActionResult> ApproveOrder(Guid orderId)
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Approved, userId);
            return Ok(order);
        }

        [HttpPut("reject/{orderId}")]
        [Authorize]
        public async Task<IActionResult> RejectOrder(Guid orderId)
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Rejected, userId);
            return Ok(order);
        }

        [HttpPut("prepare/{orderId}")]
        [Authorize]
        public async Task<IActionResult> PrepareOrder(Guid orderId)
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Prepared, userId);
            return Ok(order);
        }

        [HttpPut("ship/{orderId}")]
        [Authorize]
        public async Task<IActionResult> ShipOrder(Guid orderId)
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Shipped, userId);
            return Ok(order);
        }

        [HttpPut("deliver/{orderId}")]
        [Authorize]
        public async Task<IActionResult> DeliverOrder(Guid orderId)
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Delivered, userId);
            return Ok(order);
        }

        [HttpPut("complete/{orderId}")]
        [Authorize]
        public async Task<IActionResult> CompleteOrder(Guid orderId)
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.CompleteOrder(orderId);
            return Ok(order);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user identity");

            return userId;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            //var currentUserId = GetCurrentUserId();


            return Ok(order);
        }
        [HttpGet("orders")]
        //[Authorize]
        public async Task<IActionResult> AllOrders()
        {
            var order = await _orderService.GetAllorders();

            return Ok(order);
        }
        [HttpGet("my-orders")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = GetCurrentUserId();
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("vendor-orders")]
        //[Authorize]
        public async Task<IActionResult> GetVendorOrders([FromQuery] Guid userId)
        {
            //var vendorId = GetCurrentUserId();
            var orders = await _orderService.GetVendorOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("related-orders")]
        //[Authorize]
        public async Task<IActionResult> GetRelatedOrdersAsync([FromQuery] string orderNumber)
        {
            //var vendorId = GetCurrentUserId();
            var orders = await _orderService.GetRelatedOrdersAsync(orderNumber);
            return Ok(orders);
        }
        [HttpPost("re-assign")]
        [Authorize]
        public async Task<IActionResult> ReAssignOrder([FromBody] ReAssignOrder reAssignOrderDto)
        {
            await _orderService.ReAssignOrder(reAssignOrderDto);
            return Ok(true);
        }

        [HttpPost("remove-item")]
        [Authorize]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveItemDto removeItemDto)
        {
            await _orderService.RemoveItem(removeItemDto);
            return Ok(true);
        }

        //[HttpGet("{orderId}/transactions")]
        //public async Task<IActionResult> GetOrderTransactions(Guid orderId)
        //{
        //    var order = await _orderService.GetOrderByIdAsync(orderId);
        //    if (order == null) return NotFound();

        //    // Verify the current user has access to these transactions
        //    var currentUserId = GetCurrentUserId();
        //    if (currentUserId != order.PharmacyUserId && !User.IsInRole("Admin"))
        //    {
        //        return Forbid();
        //    }

        //    var transactions = await _balanceService.GetOrderTransactions(orderId);
        //    return Ok(transactions);
        //}
    }
}
