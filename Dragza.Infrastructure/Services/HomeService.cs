using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.EntityFrameworkCore;

namespace Dragza.Infrastructure.Services
{
    public class HomeService : IHomeService
    {
        private readonly DragzaContext _context;

        public HomeService(DragzaContext context)
        {
            _context = context;
        }

        public async Task<MobileHomeDto> GetMobileHomeAsync()
        {
            var response = new MobileHomeDto();

            var baseBannerUrl = "http://dentzone.runasp.net/uploads/banners/";
            var baseProductUrl = "http://dentzone.runasp.net/uploads/products/";
            var basecategoryUrl = "http://dentzone.runasp.net/uploads/categories/";


            // 0️⃣ Banners
            

            response.Banners = await _context.Banners
                .AsNoTracking()
                .Where(b => b.IsActive)
                .OrderBy(b => b.Order)
                .Select(b => new BannerDto
                {
                    Id = b.Id,
                    ImageName = string.IsNullOrEmpty(b.ImageName) ? null : baseBannerUrl + b.ImageName,
                    Order = b.Order
                })
                .ToListAsync();
            // 1️⃣ Categories
            response.Categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsDeleted != true)
                .Select(c => new CategoryAppDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ArabicName = c.ArabicName,
                    ImageFile = string.IsNullOrEmpty(c.ImageName) ? null : basecategoryUrl + c.ImageName
                })
                .ToListAsync();

            // 2️⃣ Top 2 Discount Products Per Inventory
            response.Inventories = await _context.ProductPrices
                .AsNoTracking()
                .Where(pp => pp.IsDeleted != true
                             && pp.StockQuantity > 0
                             && pp.DiscountRate > 0)
                .GroupBy(pp => new
                {
                    pp.InventoryUserId,
                    InventoryName = pp.InventoryUser.FullName
                })
                .Select(g => new InventorySectionDto
                {
                    InventoryId = g.Key.InventoryUserId,
                    InventoryName = g.Key.InventoryName,

                    Products = g
                        .OrderByDescending(x => x.DiscountRate)
                        .ThenByDescending(x => x.CreationDate)
                        .Select(pp => new ProductAppDto
                        {
                            ProductId = pp.ProductId,
                            ProductName = pp.Product.Name,
                            Image = string.IsNullOrEmpty(pp.Product.Image) ? null : basecategoryUrl + pp.Product.Image,
                            SalesPrice = pp.SalesPrice,
                            DiscountRate = pp.DiscountRate
                        })
                        .ToList()
                })
                .ToListAsync();

            return response;
        }
        public async Task<HomeDto> GetMobileHomeProductsAsync()
        {
            var response = new HomeDto();

            // 0️⃣ Banners
            var baseBannerUrl = "http://dentzone.runasp.net/uploads/banners/";
            var baseProductUrl = "http://dentzone.runasp.net/uploads/products/";
            var basecategoryUrl = "http://dentzone.runasp.net/uploads/categories/";

            response.Banners = await _context.Banners
                .AsNoTracking()
                .Where(b => b.IsActive)
                .OrderBy(b => b.Order)
                .Select(b => new BannerDto
                {
                    Id = b.Id,
                    ImageName = string.IsNullOrEmpty(b.ImageName) ? null : baseBannerUrl + b.ImageName,
                    Order = b.Order
                })
                .ToListAsync();

            // 1️⃣ Categories
            response.Categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.IsDeleted != true)
                .Select(c => new CategoryAppDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ArabicName = c.ArabicName,
                    ImageFile = string.IsNullOrEmpty(c.ImageName) ? null : basecategoryUrl + c.ImageName
                })
                .ToListAsync();

            // 2️⃣ All Discounted Products with InventoryUserId
            response.Products = await _context.ProductPrices
                .AsNoTracking()
                .Where(pp => pp.IsDeleted != true && pp.StockQuantity > 0 && pp.DiscountRate > 0)
                .Take(4)
                .OrderByDescending(pp => pp.DiscountRate)
                .ThenByDescending(pp => pp.CreationDate)
                .Select(pp => new ProductAppDto
                {
                    ProductId = pp.ProductId,
                    ProductName = pp.Product.Name,
                    ArabicProductName = pp.Product.ArabicName,
                    Image = string.IsNullOrEmpty(pp.Product.Image) ? null : baseProductUrl + pp.Product.Image,
                    SalesPrice = pp.SalesPrice,
                    DiscountRate = pp.DiscountRate,
                    InventoryUserId = pp.InventoryUserId,
                    Preef = pp.Product.Preef,
                    Description = pp.Product.Description,
                    ArabicDescription = pp.Product.ArabicDescription,
                    ArabicPreef = pp.Product.ArabicPreef
                    // اضفت الـ InventoryUserId لكل منتج
                })
                .ToListAsync();

            return response;
        }
    }
}