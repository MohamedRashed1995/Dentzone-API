using AutoMapper;
using Dragza.Application.Data;
using Dragza.Domain.DTO;
using Dragza.Domain.DTO.Order;
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
        private readonly IMapper _mapper;

        public CartsController(DragzaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // GET: api/cart/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .Include(c => c.Items)
                .ThenInclude(i => i.ProductPrice)
                .ThenInclude(a=>a.InventoryUser)
                .FirstOrDefaultAsync(c => c.UserId == userId);

         

          

            if (cart == null)
            {
               
                return Ok(new
                {
                    message = "Cart is empty.",
                    statusCode= StatusCodes.Status204NoContent

                });
            }
            else
            {
                cart.Items = cart.Items
                   .OrderBy(i => i.ProductPrice.InventoryUserId)
                   .ThenBy(i => i.ProductId)
                   .ToList();
                foreach (var item in cart.Items)
                {
                    item.ProductPrice.PurchasePrice=item.ProductPrice.SalesPrice-(item.ProductPrice.SalesPrice*item.ProductPrice.DiscountRate/100);
                }

            }
        
            var cartdto = _mapper.Map<CartDto>(cart);


            foreach (var item in cartdto.Items)
            {
                if (item.ProductPrice.StockQuantity <= 0)
                    item.status = 1;
            }
            //var totalsByInventory = cart.Items
            //     .GroupBy(i => i.InventoryUserId)
            //     .Select(g => new
            //     {
            //         InventoryUserId = g.Key,
            //         TotalAmount = g.Sum(item =>
            //             item.TotalAmount ?? (item.Quantity * (item.ProductPrice?.SalesPrice ?? 0)))
            //     })
            //     .ToList();
            return Ok(cartdto);
        }

        // POST: api/cart/add
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            try
            {
                double total = 0;

                var cart = await _context.Carts
          .Include(c => c.Items)
           .ThenInclude(i => i.Product)
           .Include(c => c.Items)
           .ThenInclude(i => i.InventoryUser)

          .FirstOrDefaultAsync(c => c.UserId == request.UserId);

                var porductpriceId = _context.ProductPrices.Where(a => a.InventoryUserId == request.InventoryId&&a.ProductId==request.ProductId).FirstOrDefault().Id;
                var porductprice =   _context.ProductPrices.Where(a => a.InventoryUserId == request.InventoryId && a.ProductId == request.ProductId).FirstOrDefault();
                if (cart == null)
                {

                    cart = new Cart { UserId = request.UserId };
                    _context.Carts.Add(cart);
                }

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId &&i.InventoryUserId==request.InventoryId);

                if (existingItem != null)
                {
                    existingItem.Quantity += request.Quantity;
                    existingItem.TotalAmount = (request.Quantity * (porductprice.SalesPrice-(porductprice.SalesPrice*porductprice.DiscountRate/100)));
                }
                else
                    cart.Items.Add(new CartItem
                    {
                        InventoryUserId = request.InventoryId,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        ProductPriceId = porductpriceId,
                        TotalAmount = (request.Quantity * (porductprice.SalesPrice - (porductprice.SalesPrice * porductprice.DiscountRate / 100)))

                    });
                foreach (var item in cart.Items) {
                    total += (double)item.TotalAmount;
                }

                cart.TotalAmountCart = total;
                await _context.SaveChangesAsync();
                return Ok(cart);
            }
            catch (Exception ex)
            {

                throw;
            }
      
        }


        // PUT: api/cart/updateQuantity
        [HttpPut("UpdateCartQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartQuantityRequestDto request)
        {
            double total = 0;

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .Include(c => c.Items)
                .ThenInclude(i => i.InventoryUser)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);
            
            var porductpriceId = _context.ProductPrices.Where(a => a.InventoryUserId == request.InventoryId && a.ProductId == request.ProductId).FirstOrDefault().Id;
            var porductprice = _context.ProductPrices.Where(a => a.InventoryUserId == request.InventoryId && a.ProductId == request.ProductId).FirstOrDefault();

            if (cart == null)
                return NotFound(new { message = "Cart not found." });

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId&&i.InventoryUserId==request.InventoryId);
            
            if (item == null)
                return NotFound(new { message = "Item not found in cart." });

            if (request.Quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = request.Quantity;
                item.TotalAmount = (request.Quantity * (porductprice.SalesPrice - (porductprice.SalesPrice * porductprice.DiscountRate / 100)));
            }

            foreach (var itm in cart.Items)
            {
                total += (double)itm.TotalAmount;
            }

            cart.TotalAmountCart = total;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cart updated successfully.", cart });
        }


        // DELETE: api/cart/remove
        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequestDto request)
        {
            decimal? total = 0;
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null)
                return NotFound();

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item == null)
                return NotFound();

            cart.Items.Remove(item);
          
            foreach (var itm in cart.Items)
            {
                total += itm.TotalAmount;
            }
            cart.TotalAmountCart = (double)total;
            await _context.SaveChangesAsync();
            return Ok(cart);
        }


    }
}
