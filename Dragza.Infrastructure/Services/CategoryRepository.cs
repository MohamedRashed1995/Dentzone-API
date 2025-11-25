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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly DragzaContext _context;

        public CategoryRepository(DragzaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllWithProductsAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();
        }

        public async Task<Category> GetByName(string name)
        {
            var category =  await _context.Categories.Where(a => a.ArabicName == name || a.Name == name).FirstOrDefaultAsync();
            return  category;
        }
    }
}
