using AutoMapper;
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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly IMapper _mapper;
        public ProductRepository(DragzaContext context ) : base(context) 
        {
        }

        public async Task<Product> GetProductWithDetailsAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ActiveIngerdient)
                .Include(p => p.ProductPrices)
                    .ThenInclude(pp => pp.InventoryUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetAllProductsWithDetailsAsync(bool includeDeleted , string search="", int page = 1, int size = 10)
        {
            try
            {
               var query = _context.Products
              .Where(p => p.IsDeleted != true)
              .Include(p => p.Category)
              .Include(p => p.ActiveIngerdient)
              .Include(p => p.ProductPrices)
                  .ThenInclude(pp => pp.InventoryUser)
              .AsQueryable();
                var totalrow = query.Count();
              
                var pages = (int)Math.Ceiling((decimal)totalrow / size);
             

                query = query.Skip((page - 1) * size).Take(size);


                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(p => p.Name.Contains(search) || p.ArabicName.Contains(search) || p.ActiveIngerdient.Name.Contains(search));

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }
        public async Task<List<Product>> GetByActiveIngredientAsync(Guid activeIngredientId)
        {
            return await _context.Products
                .Where(p => p.ActiveIngerdientId != null && p.ActiveIngerdientId == activeIngredientId && p.IsDeleted != true)
                .Include(p => p.Category)
                .Include(p => p.ActiveIngerdient)
                .Include(p => p.ProductPrices)
                    .ThenInclude(pp => pp.InventoryUser)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetCompletedOrderIdsAsync()
        {
            return await _context.Orders
                .Where(o => o.Status >= 5) // Adjust status codes as per your business logic
                .Select(o => o.Id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ActiveIngerdient)
                .Include(a=>a.ProductPrices)
                .Where(p => p.CategoryId == categoryId && (p.IsDeleted == null || p.IsDeleted == false))
                .ToListAsync();
        }
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

        public async Task<List<ProductPrice>> GetPricesWithDetailsByCategory(Guid categoryId)
        {
            return await _context.ProductPrices
                .Where(pp => pp.CategoryId == categoryId && pp.IsDeleted != true)
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .Include(pp => pp.InventoryUser)
                .OrderByDescending(pp => pp.CreationDate)
                .ToListAsync();
        }

        public async Task<List<Product>> GetPricesWithAllProductByInventoryId(Guid inventoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ActiveIngerdient)
                .Include(p => p.ProductPrices)
                    .ThenInclude(pp => pp.InventoryUser)
                .Where(p => p.IsDeleted != true && p.ProductPrices.Any(x => x.InventoryUserId == inventoryId))
                .ToListAsync();

            return products;

        }
    }
}
