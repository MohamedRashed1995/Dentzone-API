using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class MainCategoryService : IMainCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MainCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MainCategoryWithRelationsDto> GetByIdAsync(Guid id)
        {
            var mainCategory = await _unitOfWork.MainCategoryRepository.GetByIdAsync(id);
            return _mapper.Map<MainCategoryWithRelationsDto>(mainCategory);
        }

        public async Task<IEnumerable<MainCategoryDto>> GetAllAsync()
        {
            var mainCategories = await _unitOfWork.MainCategoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MainCategoryDto>>(mainCategories);
        }

        public async Task<MainCategoryDto> CreateAsync(CreateMainCategoryDto createDto)
        {
            var mainCategory = _mapper.Map<MainCategory>(createDto);
            await  _unitOfWork.MainCategoryRepository.AddAsync(mainCategory);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MainCategoryDto>(mainCategory);
        }

        public async Task UpdateAsync(Guid id, UpdateMainCategoryDto updateDto)
        {
            var existing = await _unitOfWork.MainCategoryRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("MainCategory not found");

            _mapper.Map(updateDto, existing);
             _unitOfWork.MainCategoryRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _unitOfWork.MainCategoryRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("MainCategory not found");
             _unitOfWork.MainCategoryRepository.Delete(existing);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(Guid mainCategoryId)
        {
            var categories = await _unitOfWork.MainCategoryRepository.GetCategoriesByMainCategoryAsync(mainCategoryId);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(Guid mainCategoryId)
        {
            var products = await _unitOfWork.MainCategoryRepository.GetProductsByMainCategoryAsync(mainCategoryId);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
