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
		public async Task<IEnumerable<ProductPrice>> GetAllProductPricesWithDetailsAsync(bool includeDeleted, string search)
		{
			var query = _context.ProductPrices
				.Include(pp => pp.Product)
					.ThenInclude(p => p.Category)
				.Include(pp => pp.Product)
					.ThenInclude(p => p.ActiveIngerdient)
				.Include(pp => pp.Category)
				.Include(pp => pp.InventoryUser)
				.Include(pp => pp.MainCategory)
				.Where(pp => includeDeleted || pp.IsDeleted != true);

			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(pp =>
					pp.Product.Name.Contains(search) ||
					pp.Product.ArabicName.Contains(search) ||
					pp.Product.Description.Contains(search));
			}

			return await query
				.OrderBy(pp => pp.Product.Name)
				.ThenByDescending(pp => pp.CreationDate)
				.ToListAsync();
		}
	}
}
