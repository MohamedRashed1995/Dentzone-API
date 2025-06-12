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
    // ProductPriceRepository.cs
    public class ProductPriceRepository : Repository<ProductPrice>, IProductPriceRepository
    {
        private readonly DragzaContext _context;

        public ProductPriceRepository(DragzaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductPrice>> GetPricesByProductAsync(Guid productId)
        {
            return await _context.ProductPrices
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .Include(pp => pp.InventoryUser)
                .Where(pp => pp.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductPrice>> GetPricesByInventoryUserAsync(Guid userId)
        {
            return await _context.ProductPrices
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .Include(pp => pp.InventoryUser)
                .Where(pp => pp.InventoryUserId == userId)
                .ToListAsync();
        }

        public async Task<List<ProductPrice>> GetBestPricesAsync()
        {
            return await _context.ProductPrices
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .Include(pp => pp.InventoryUser)
                .Where(pp => pp.IsDeleted != true)
                .GroupBy(pp => pp.ProductId)
                .Select(g => g.OrderBy(pp => pp.SalesPrice).FirstOrDefault())
                .ToListAsync();
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

        public async Task<List<ProductPrice>> GetPricesByProduct(Guid productId)
        {
            return await _context.ProductPrices
                .Where(pp => pp.ProductId == productId && pp.IsDeleted != true)
                .Include(pp => pp.Category)
                .Include(pp => pp.InventoryUser)
                .OrderByDescending(pp => pp.CreationDate)
                .ToListAsync();
        }

        public async Task<List<ProductPrice>> GetPricesByInventoryUser(Guid userId)
        {
            return await _context.ProductPrices
                .Where(pp => pp.InventoryUserId == userId && pp.IsDeleted != true)
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .OrderByDescending(pp => pp.CreationDate)
                .ToListAsync();
        }

        public async Task<ProductPrice> GetPricesByInventoryAndProduct(Guid userId , Guid productId)
        {
            return await _context.ProductPrices
                .Where(pp => pp.InventoryUserId == userId && pp.ProductId == productId &&pp.IsDeleted != true)
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .OrderByDescending(pp => pp.CreationDate)
                .FirstOrDefaultAsync();
        }
    }
}
