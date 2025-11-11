using Dragza.Application.Data;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dragza.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly DragzaContext _context;

        public CartsController(DragzaContext context)
        {
            _context = context;
        }


        // GET: api/cart/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(a=>a.ProductPrices)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }

        // POST: api/cart/add
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                 .ThenInclude(i => i.Product)
                 .Include(c => c.Items)
                 .ThenInclude(i => i.InventoryUser)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null)
            {
                cart = new Cart { UserId = request.UserId };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (existingItem != null)
                existingItem.Quantity += request.Quantity;
            else
                cart.Items.Add(new CartItem { InventoryUserId=request.InventoryId, ProductId = request.ProductId, Quantity = request.Quantity });

            await _context.SaveChangesAsync();
            return Ok(cart);
        }

        // DELETE: api/cart/remove
        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequestDto request)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null)
                return NotFound();

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item == null)
                return NotFound();

            cart.Items.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(cart);
        }


    }
}
