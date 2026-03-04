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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                .Include(pp => pp.Inventory)
                .Where(pp => pp.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductPrice>> GetPricesByInventoryUserAsync(Guid userId)
        {
            return await _context.ProductPrices
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .Include(pp => pp.Inventory)
                .Where(pp => pp.InventoryUserId == userId)
                .ToListAsync();
        }

        public async Task<List<ProductPrice>> GetBestPricesAsync(int page = 1, int size = 10)
        {
          
         
           var productPrices=  await _context.ProductPrices
                .Include(pp => pp.Product)
             
                .Include(pp => pp.Inventory)
                .Where(pp => pp.IsDeleted != true)
                .Skip((page - 1) * size).Take(size)
                .GroupBy(pp => pp.ProductId)
                .Select(g => g.OrderBy(pp => pp.SalesPrice).FirstOrDefault())
                .ToListAsync();

            var totalrow = productPrices.Count();
            var pages = (int)Math.Ceiling((decimal)totalrow / size);
            return productPrices;




        }

        public async Task<List<ProductPrice>> GetBestPricesSortingAsync(int sort ,int page = 1, int size = 10)
        {




            var productPricesData = await _context.ProductPrices
                  .AsNoTracking()
                     .Include(pp => pp.Product)
                     .Include(pp => pp.Inventory)
                     .Where(pp => pp.IsDeleted != true && pp.StockQuantity > 0)
                    
                     .ToListAsync();

            var productPrices = productPricesData
    .GroupBy(pp => pp.ProductId)
    .Select(g => g.OrderBy(pp => pp.DiscountRate).FirstOrDefault());

            if (sort == 1)
            {
                productPrices = productPricesData.OrderByDescending(s => s.DiscountRate);
            }
            if (sort == 2)
            {
                productPrices = productPricesData.OrderBy(s => s.DiscountRate);
            }


            return productPrices.Skip((page - 1) * size).Take(size).ToList();








        }


        public async Task<List<ProductPrice>> GetPricesWithDetailsByCategory(Guid categoryId)
        {
            return await _context.ProductPrices
                .Where(pp => pp.CategoryId == categoryId && pp.IsDeleted != true)
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .Include(pp => pp.Inventory)
                .OrderByDescending(pp => pp.CreationDate)
                .ToListAsync();
        }

        public async Task<List<ProductPrice>> GetPricesByProduct(Guid productId)
        {
            return await _context.ProductPrices
                .Where(pp => pp.ProductId == productId && pp.IsDeleted != true)
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .Include(pp => pp.Inventory)
                .OrderByDescending(pp => pp.CreationDate)
                .ToListAsync();
        }

        public async Task<List<ProductPrice>> GetPricesByInventoryUser(Guid userId)
        {
            return await _context.ProductPrices
                .Where(pp => pp.InventoryUserId == userId && pp.IsDeleted != true)
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .Include (pp => pp.Inventory)
                .OrderByDescending(pp => pp.CreationDate)
                .ToListAsync();
        }

        public async Task<ProductPrice> GetPricesByInventoryAndProduct(Guid userId , Guid productId)
        {
            return await _context.ProductPrices
                .Where(pp => pp.InventoryUserId == userId && pp.ProductId == productId &&pp.IsDeleted != true)
                .Include(pp => pp.Product)
                .Include(pp => pp.Category)
                .Include(pp => pp.Inventory)
                .OrderByDescending(pp => pp.CreationDate)
                .FirstOrDefaultAsync();
        }
		public async Task<IEnumerable<ProductPrice>> GetAllProductPricesWithDetailsAsync(bool includeDeleted, string search , int page = 1, int size = 10)
		{
            var query = _context.ProductPrices
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Category)
                .Include(pp => pp.Product)
                    //.ThenInclude(p => p.ActiveIngerdient)
                .Include(pp => pp.Category)
                .Include(pp => pp.Inventory)
                //.Include(pp => pp.MainCategory)
                .Where(pp => includeDeleted || pp.IsDeleted != true&&pp.StockQuantity>0);

            

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(pp =>
                        pp.Product.Name.Contains(search) ||
                        pp.Product.ArabicName.Contains(search) ||
                        pp.Product.Description.Contains(search));
                 
                 }

            query = query.OrderByDescending(pp => pp.DiscountRate);

            query = query.Skip((page - 1) * size).Take(size);
            return await query.ToListAsync();

            //return await query
            //        .OrderBy(pp => pp.Product.Name)
            //        .ThenByDescending(a=>a.DiscountRate)
            //        .ToListAsync();

        }

        public async Task<List<ProductPrice>> GetProductByCategory(Guid categoryId)
        {
            var productPricesData = await _context.ProductPrices
                   .Include(pp => pp.Product)
                   .Include(pp => pp.Inventory)
                   .Where(pp => pp.IsDeleted != true &&pp.Product.CategoryId==categoryId&&pp.StockQuantity>0)

                   .ToListAsync();

            return productPricesData;

         
        }

        public async Task<IEnumerable<ProductPrice>> GetAllProductAsync(Guid productId)
        {
            var query = _context.ProductPrices
                .Include(pp => pp.Product)
                    .ThenInclude(p => p.Category)
                .Include(pp => pp.Product)
                    //.ThenInclude(p => p.ActiveIngerdient)
                .Include(pp => pp.Category)
                .Include(pp => pp.Inventory)
                //.Include(pp => pp.MainCategory)
                .Where(pp =>pp.IsDeleted != true&&pp.ProductId==productId&&pp.StockQuantity>0);

            query= query.OrderByDescending(pp => pp.DiscountRate);

            return query.ToList();
        }

        public async Task<IEnumerable<ProductPrice>> GetPricesByIdAsync(Guid productPriceId)
        {


            var result = _context.ProductPrices
                .Where(a => a.Id == productPriceId).ToList();
            return  result;



           //var query = _context.ProductPrices
           //     .Include(pp => pp.Product)
           //         .ThenInclude(p => p.Category)
           //     .Include(pp => pp.Product)
           //         .ThenInclude(p => p.ActiveIngerdient)
           //     .Include(pp => pp.Category)
           //     .Include(pp => pp.InventoryUser)
           //     .Include(pp => pp.MainCategory)
           //     .Where(pp => pp.Id == productPriceId && pp.IsDeleted != true);
           // return Task.FromResult(query.AsEnumerable());
        }
    }
}
