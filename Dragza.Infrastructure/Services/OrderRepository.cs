using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    // OrderRepository.cs
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(DragzaContext context) : base(context) { }

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
