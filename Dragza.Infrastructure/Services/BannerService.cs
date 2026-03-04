//using Dragza.Application.Data;
//using Dragza.Application.Interface;
//using Dragza.Domain.DTO;
//using Dragza.Domain.Models;
//using Dragza.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;

//namespace Dragza.Infrastructure.Services
//{
//    public class BannerService : IBannerService
//    {
//        private readonly DragzaContext _context;
//        private readonly string _uploadsFolder;
//        private readonly string _baseUrl = "http://dentzone.runasp.net/Uploads/banners/";

//        public BannerService(DragzaContext context)
//        {
//            _context = context;
//            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "banners");
//            if (!Directory.Exists(_uploadsFolder))
//                Directory.CreateDirectory(_uploadsFolder);
//        }

//        // Get All Banners
//        public async Task<IEnumerable<BannerDto>> GetAllAsync()
//        {
//            return await _context.Banners
//                .Where(b => b.IsActive)
//                .OrderBy(b => b.Order)
//                .Select(b => new BannerDto
//                {
//                    Id = b.Id,
//                    ImageName = string.IsNullOrEmpty(b.ImageName) ? null : _baseUrl + b.ImageName,
//                    Order = b.Order
//                })
//                .ToListAsync();
//        }

//        // Get By Id
//        public async Task<BannerDto?> GetByIdAsync(Guid id)
//        {
//            var banner = await _context.Banners.FindAsync(id);
//            if (banner == null) return null;

//            return new BannerDto
//            {
//                Id = banner.Id,
//                ImageName = string.IsNullOrEmpty(banner.ImageName) ? null : _baseUrl + banner.ImageName,
//                Order = banner.Order
//            };
//        }

//        // Create Banner
//        public async Task<BannerDto> CreateAsync(BannerDto dto)
//        {
//            string? imageName = null;
//            if (dto.ImageFile != null)
//            {
//                imageName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
//                var filePath = Path.Combine(_uploadsFolder, imageName);
//                using var stream = new FileStream(filePath, FileMode.Create);
//                await dto.ImageFile.CopyToAsync(stream);
//            }

//            var banner = new Banner
//            {
//                Id = Guid.NewGuid(),
//                ImageName = imageName,
//                Order = dto.Order,
//                IsActive = true,
//                CreatedAt = DateTime.UtcNow
//            };

//            _context.Banners.Add(banner);
//            await _context.SaveChangesAsync();

//            dto.Id = banner.Id;
//            dto.ImageName = imageName != null ? _baseUrl + imageName : null;
//            return dto;
//        }

//        // Update Banner
//        public async Task<bool> UpdateAsync(Guid id, BannerDto dto)
//        {
//            var banner = await _context.Banners.FindAsync(id);
//            if (banner == null) return false;

//            if (dto.ImageFile != null)
//            {
//                // حذف الصورة القديمة
//                if (!string.IsNullOrEmpty(banner.ImageName))
//                {
//                    var oldFile = Path.Combine(_uploadsFolder, Path.GetFileName(banner.ImageName));
//                    if (File.Exists(oldFile)) File.Delete(oldFile);
//                }

//                // رفع الصورة الجديدة
//                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
//                var filePath = Path.Combine(_uploadsFolder, imageName);
//                using var stream = new FileStream(filePath, FileMode.Create);
//                await dto.ImageFile.CopyToAsync(stream);

//                banner.ImageName = imageName;
//            }

//            banner.Order = dto.Order;
//            _context.Entry(banner).State = EntityState.Modified;
//            await _context.SaveChangesAsync();
//            return true;
//        }

//        // Delete Banner
//        public async Task<bool> DeleteAsync(Guid id)
//        {
//            var banner = await _context.Banners.FindAsync(id);
//            if (banner == null) return false;

//            // حذف الصورة من السيرفر
//            if (!string.IsNullOrEmpty(banner.ImageName))
//            {
//                var oldFile = Path.Combine(_uploadsFolder, Path.GetFileName(banner.ImageName));
//                if (File.Exists(oldFile)) File.Delete(oldFile);
//            }

//            _context.Banners.Remove(banner);
//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//}



using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Dragza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Dragza.Infrastructure.Services
{
    public class BannerService : IBannerService
    {
        private readonly DragzaContext _context;
        private readonly string _uploadsFolder;
        private readonly string _baseUrl = "http://dentzone.runasp.net/Uploads/banners/";

        public BannerService(DragzaContext context)
        {
            _context = context;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "banners");
            if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);
        }

        // Get All Banners
        public async Task<IEnumerable<BannerDto>> GetAllAsync()
        {
            return await _context.Banners
                .Where(b => b.IsActive)
                .OrderBy(b => b.Order)
                .Select(b => new BannerDto
                {
                    Id = b.Id,
                    ImageName = string.IsNullOrEmpty(b.ImageName) ? null : $"{_baseUrl}{b.ImageName}",
                    Order = b.Order
                })
                .ToListAsync();
        }

        // Get By Id
        public async Task<BannerDto?> GetByIdAsync(Guid id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) return null;

            return new BannerDto
            {
                Id = banner.Id,
                ImageName = string.IsNullOrEmpty(banner.ImageName) ? null : $"{_baseUrl}{banner.ImageName}",
                Order = banner.Order
            };
        }

        // Create Banner
        public async Task<BannerDto> CreateAsync(BannerDto dto)
        {
            string? imageName = null;
            if (dto.ImageFile != null)
            {
                imageName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var filePath = Path.Combine(_uploadsFolder, imageName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.ImageFile.CopyToAsync(stream);
            }

            var banner = new Banner
            {
                Id = Guid.NewGuid(),
                ImageName = imageName, // خزن الاسم فقط
                Order = dto.Order,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            dto.Id = banner.Id;
            dto.ImageName = imageName != null ? $"{_baseUrl}{imageName}" : null; // أرسل الرابط الكامل للعميل
            return dto;
        }

        // Update Banner
        public async Task<bool> UpdateAsync(Guid id, BannerDto dto)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) return false;

            if (dto.ImageFile != null)
            {
                // حذف الصورة القديمة
                if (!string.IsNullOrEmpty(banner.ImageName))
                {
                    var oldFile = Path.Combine(_uploadsFolder, Path.GetFileName(banner.ImageName));
                    if (File.Exists(oldFile)) File.Delete(oldFile);
                }

                // رفع الصورة الجديدة
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var filePath = Path.Combine(_uploadsFolder, imageName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.ImageFile.CopyToAsync(stream);

                banner.ImageName = imageName;
            }

            banner.Order = dto.Order;
            _context.Entry(banner).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // Delete Banner
        public async Task<bool> DeleteAsync(Guid id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) return false;

            // حذف الصورة من السيرفر
            if (!string.IsNullOrEmpty(banner.ImageName))
            {
                var oldFile = Path.Combine(_uploadsFolder, Path.GetFileName(banner.ImageName));
                if (File.Exists(oldFile)) File.Delete(oldFile);
            }

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}