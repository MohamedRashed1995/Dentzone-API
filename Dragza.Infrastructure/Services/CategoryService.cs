using AutoMapper;
using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Dragza.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly DragzaContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _uploadsFolder;
        private readonly string _baseUrl = "http://dentzone.runasp.net/Uploads/categories/";

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, DragzaContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "categories");
            if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            var result = _mapper.Map<IEnumerable<CategoryDto>>(categories).ToList();

            foreach (var item in result)
            {
                var category = categories.First(x => x.Id == item.Id);

                if (!string.IsNullOrEmpty(category.ImageName))
                    item.ImageName = _baseUrl + category.ImageName;
            }

            return result;
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            var dto = _mapper.Map<CategoryDto>(category);

            if (!string.IsNullOrEmpty(category.ImageName))
                dto.ImageName = _baseUrl + category.ImageName;

            return dto;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.Id = Guid.NewGuid();
            category.CreatedAt = DateTime.UtcNow;
            string? imageName = null;
            if (categoryDto.ImageFile != null)
            {
                imageName = $"{Guid.NewGuid()}{Path.GetExtension(categoryDto.ImageFile.FileName)}";
                var filePath = Path.Combine(_uploadsFolder, imageName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await categoryDto.ImageFile.CopyToAsync(stream);
                category.ImageName = imageName;
                //category.Image
            }
            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(category);
        }


        public async Task UpdateCategoryAsync(Guid id, CreateCategoryDto categoryDto)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found");

            // تحديث البيانات الأساسية
            category.Name = categoryDto.Name;
            category.Pref = categoryDto.Pref;
            category.Description = categoryDto.Description;
            category.ArabicName = categoryDto.ArabicName;
            category.UpdatedAt = DateTime.UtcNow;

            // 👇 لو فيه صورة جديدة
            if (categoryDto.ImageFile != null)
            {
                // حذف الصورة القديمة لو موجودة
                if (!string.IsNullOrEmpty(category.ImageName))
                {
                    var oldImagePath = Path.Combine(_uploadsFolder, category.ImageName);
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                // رفع الصورة الجديدة
                var newImageName = $"{Guid.NewGuid()}{Path.GetExtension(categoryDto.ImageFile.FileName)}";
                var newImagePath = Path.Combine(_uploadsFolder, newImageName);

                using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                    await categoryDto.ImageFile.CopyToAsync(stream);
                }

                category.ImageName = newImageName;
            }

            try
            {
                _unitOfWork.CategoryRepository.Update(category);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ApplicationException("Concurrency error occurred while updating category", ex);
            }
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found");
            }

            try
            {
                _unitOfWork.CategoryRepository.Delete(category);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("Error occurred while deleting category", ex);
            }
        }

        public Task<Category> GetCategoryByNameAsync(string name)
        {
            var category = _unitOfWork.CategoryRepository.GetByName(name);
            return category;
        }

        // Implement other methods similarly
    }
}
