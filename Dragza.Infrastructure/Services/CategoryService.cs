using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(include: q => q.Include(o => o.MainCategory));
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAllAsync(o => o.Id == id, include: q => q.Include(o => o.MainCategory));
            return _mapper.Map<CategoryDto>(category.FirstOrDefault());
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.Id = Guid.NewGuid();
            category.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(category);
        }


        public async Task UpdateCategoryAsync(Guid id, CreateCategoryDto categoryDto)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found");
            }

            // Update category properties
            _mapper.Map(categoryDto, category);
            category.UpdatedAt = DateTime.UtcNow;
            category.Id = id; // Ensure the ID remains the same

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
