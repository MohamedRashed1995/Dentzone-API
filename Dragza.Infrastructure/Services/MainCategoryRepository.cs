using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class MainCategoryRepository : Repository<MainCategory>, IMainCategoryRepository
    {
        public MainCategoryRepository(DragzaContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetCategoriesByMainCategoryAsync(Guid mainCategoryId)
        {
            return await _context.Categories
                .Where(c => c.MainCategoryId == mainCategoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByMainCategoryAsync(Guid mainCategoryId)
        {
            return await _context.Products
                .Where(p => p.MainCategoryId == mainCategoryId)
                .Include(p => p.Category)
                .ToListAsync();
        }
    }
}
