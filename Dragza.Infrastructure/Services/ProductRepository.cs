using AutoMapper;
using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Dragza.Infrastructure.Services
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly string _baseProductUrl = "http://dentzone.runasp.net/Uploads/products/";
        private readonly string _baseCategoriesUrl = "http://dentzone.runasp.net/Uploads/categories/";


        public ProductRepository(DragzaContext context ) : base(context) 
        {
           
        }

        public async Task<Product> GetProductWithDetailsAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                //.Include(p => p.BestSellerProducts)
                .Include(p => p.ProductPrices)
                    .ThenInclude(pp => pp.InventoryUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetAllProductsWithDetailsAsync(bool includeDeleted, string search = "", int page = 1, int size = 10)
        {
            var query = _context.Products.AsQueryable();

            

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || p.ArabicName.Contains(search));

            // Include قبل Skip/Take
            query = query
                .Include(p => p.Category)
                .Include(p => p.ProductPrices)
                    .ThenInclude(pp => pp.InventoryUser)
                .AsSplitQuery()
                .OrderByDescending(p => p.CreatedAt);

            var data = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Include(p => p.Category)
                .Include(p => p.ProductPrices)
                    .ThenInclude(pp => pp.InventoryUser)
                .AsSplitQuery()
                .ToListAsync();

            // تعديل الصور قبل الإرجاع
            data.ForEach(p =>
            {
                // للـ Product
                if (!string.IsNullOrEmpty(p.Image))
                {
                    if (!p.Image.StartsWith("http"))
                    {
                        var imageFile = p.Image;
                        if (imageFile.StartsWith("products/"))
                            imageFile = imageFile.Substring(9);

                        p.Image = $"{_baseProductUrl}{imageFile}";
                    }
                }

                // للـ Category
                if (p.Category != null && !string.IsNullOrEmpty(p.Category.ImageName))
                {
                    if (!p.Category.ImageName.StartsWith("http"))
                    {
                        p.Category.ImageName = $"{_baseCategoriesUrl}{p.Category.ImageName}";
                    }
                }
            });

            return data;
        }
        //public async Task<List<Product>> GetByActiveIngredientAsync(Guid activeIngredientId)
        //{
        //    return await _context.Products
        //        .Where(p => p.ActiveIngerdientId != null && p.ActiveIngerdientId == activeIngredientId && p.IsDeleted != true)
        //        .Include(p => p.Category)
        //        //.Include(p => p.ActiveIngerdient)
        //        .Include(p => p.ProductPrices)
        //            .ThenInclude(pp => pp.InventoryUser)
        //        .ToListAsync();
        //}

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
                .Include(p => p.ProductPrices)
                    .ThenInclude(pp => pp.InventoryUser)
                        .ThenInclude(u => u.Addresses) // جلب العناوين
                .Where(p => p.CategoryId == categoryId && p.ProductPrices.Any())
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
                .Where(pp => pp.Product.CategoryId == categoryId && pp.IsDeleted != true)
                .Include(pp => pp.Product)
                .Include(pp => pp.Product.Category)
                .Include(pp => pp.InventoryUser)
                .OrderByDescending(pp => pp.CreationDate)
                .ToListAsync();
        }

        public async Task<List<Product>> GetPricesWithAllProductByInventoryId(Guid inventoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                //.Include(p => p.ActiveIngerdient)
                .Include(p => p.ProductPrices)
                    .ThenInclude(pp => pp.InventoryUser)
                .Where(p => p.ProductPrices.Any(x => x.InventoryUserId == inventoryId))
                .ToListAsync();

            return products;

        }

        public async Task<Product> GetProductByName(string name)
        {
            var product = await _context.Products.Include(a=>a.ProductPrices)
                .Where(a => a.Name.Contains(name) || a.ArabicName.Contains(name)).FirstOrDefaultAsync();
              

            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string name)
        {
            try
            {
                var products = await _context.Products.Include(a => a.ProductPrices)
               .Where(a => a.Name.StartsWith(name) || a.ArabicName.StartsWith(name)).ToListAsync();

                products = products.Where(a => a.ProductPrices.Count > 0).ToList();
 
                return products;
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }

        public async Task<Product> AddProduct(Product product)
        {
            try
            {
               
                 _context.Products.Add(product);
                  _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Product> GetProductByCode(int productCode)
        {
            var product = await _context.Products.Include(a => a.ProductPrices)
               .Where(a=>a.ProductCode==productCode).FirstOrDefaultAsync();


            return product;
        }
    }
}
