using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Dragza.Infrastructure.Services
{
    // OrderRepository.cs
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(DragzaContext context) : base(context) { }

        public async Task<(bool Success, string Message)> CreateOrderById(string userId)
        {
            try
            {
                var failedInventory = new List<string>();
                decimal? total = 0;


                var cart = await _context.Carts
               .Include(c => c.Items)
               .ThenInclude(i => i.ProductPrice)
                 .Include(c => c.Items)
               .ThenInclude(i => i.InventoryUser)
               .FirstOrDefaultAsync(c => c.UserId == userId);

                var totalInventories = cart.Items.Select(i => i.InventoryUserId).Distinct().Count();

                if (cart != null)
                {
                    var groups = cart.Items.GroupBy(i => i.InventoryUserId);


                    foreach (var group in groups)
                    {
                        var warehouseId = group.Key;
                        var checkMinOrder = _context.Users.Where(a=>a.Id==warehouseId).FirstOrDefault().MinOrder;
                        var totalAmount = (decimal)group.Sum(i => i.Quantity * i.ProductPrice.SalesPrice);
                        if (totalAmount >= checkMinOrder) {
                            var order = new Order
                            {
                                Id = Guid.NewGuid(),
                                PharmacyUserId = Guid.Parse(userId),
                                InventoryUserId = warehouseId,
                                OrderDate = DateTime.Now,
                                Status = (int)OrderStatus.Pending,
                                TotalAmount = (decimal)group.Sum(i => i.Quantity * i.ProductPrice.SalesPrice),
                                OrderNumber = Guid.NewGuid().ToString().Substring(0, 8)
                            };

                            _context.Orders.Add(order);
                            await _context.SaveChangesAsync();

                            foreach (var item in group)
                            {
                                var orderItem = new OrderItem
                                {
                                    Id = Guid.NewGuid(),
                                    OrderId = order.Id,
                                    ProductId = item.ProductId,
                                    ProductPriceId = item.ProductPriceId,
                                    Quantity = item.Quantity,
                                    Amount = (decimal)(item.Quantity * item.ProductPrice.SalesPrice),
                                    InventoryId = warehouseId
                                };

                                _context.OrderItems.Add(orderItem);
                                await _context.SaveChangesAsync();

                            }
                            var myCart = cart.Items.Where(a => a.InventoryUserId == warehouseId).ToList();
                            _context.CartItems.RemoveRange(myCart);
                            await _context.SaveChangesAsync();


                            if (cart.Items.Count == 0)
                            {
                                _context.Carts.Remove(cart);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                foreach (var itm in cart.Items)
                                {
                                    total += itm.TotalAmount;
                                }
                                cart.TotalAmountCart = (double)total;
                                await _context.SaveChangesAsync();

                            }

                            


                        }
                        else
                        {

                            failedInventory.Add(
                                   $"المخزن {cart.Items.Where(a=>a.InventoryUserId==warehouseId).FirstOrDefault().InventoryUser.BussinesName}: الحد الأدنى {checkMinOrder} — المجموع {totalAmount}");
                            continue;  // كمل على باقي المخازن


                        }


                       


                    }

                    if (failedInventory.Count == totalInventories && totalInventories > 0)
                    {
                        return (false, "لم يصل الي الحد الادني للطلب من اي مخزن");
                    }


                    if (failedInventory.Any())
                        {
                            var message = "تم إنشاء الطلبات بنجاح لبعض المخازن." +
                               Environment.NewLine +
                               "لم يتم إنشاء طلب للمخازن التالية:" +
                               Environment.NewLine +
                               string.Join(Environment.NewLine, failedInventory);


                            return (true, message);
                        }
                        else
                        {
                            return (true, "تم إنشاء جميع الطلبات بنجاح.");
                        }

                    // _context.CartItems.RemoveRange(cart.Items);
                 

                    // تجهيز الرسالة النهائية
                  

                }
                else
                {
                    return (false, "Cart not found");
                }
                
            }
            catch (Exception ex)
            {
                return (false, ex.Message);

            }
                      


        }

        public async Task<Order> GetByIdWithItemsAsync(Guid id)
        {
            return await _context.Orders
                .Include(u => u.InventoryUser)
                .Include(p => p.PharmacyUser)
                .Include(cu => cu.CouponUsages)
                .ThenInclude(c=> c.Coupon)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.ProductPrice)
                .ThenInclude(n => n.InventoryUser)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductPrice)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<List<Guid>> GetCompletedOrderIdsAsync()
        {
            return await _context.Orders
                .Where(o => o.Status >= 5) // Adjust status codes as per your business logic
                 .Include(u => u.InventoryUser)
                .Include(p => p.PharmacyUser)
                .Include(cu => cu.CouponUsages)
                .ThenInclude(c => c.Coupon)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductPrice)
                .Select(o => o.Id)
                .ToListAsync();
        }

    }

    // OrderItemRepository.cs
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(DragzaContext context) : base(context) { }


        public IQueryable<BestSellerProduct> GetBestSellingProductsQuery(List<Guid> orderIds)
        {
            return _context.OrderItems
                .Where(oi => orderIds.Contains(oi.OrderId))
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new BestSellerProduct
                {
                    ProductId = g.Key.ProductId,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Amount)
                })
                .OrderByDescending(x => x.TotalQuantitySold);
        }

        public async Task<List<BestSellerProduct>> GetBestSellingProductsAsync(int topN)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .ThenInclude(x => x.ProductPrices)
                .Where(oi => oi.Order.Status >= 5) // Completed orders
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new BestSellerProduct
                {
                    ProductId = g.Key.ProductId,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Amount)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(topN)
                .ToListAsync();
        }

		public async Task<IEnumerable<OrderItem>> GetItemsByOrderIdAsync(Guid orderId)
		{
			return await _context.OrderItems
				.Where(oi => oi.OrderId == orderId)
				.ToListAsync();
		}
	}
}
