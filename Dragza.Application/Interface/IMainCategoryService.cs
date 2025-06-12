using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IMainCategoryService
    {
        Task<MainCategoryWithRelationsDto> GetByIdAsync(Guid id);
        Task<IEnumerable<MainCategoryDto>> GetAllAsync();
        Task<MainCategoryDto> CreateAsync(CreateMainCategoryDto createDto);
        Task UpdateAsync(Guid id, UpdateMainCategoryDto updateDto);
        Task DeleteAsync(Guid id);

        //Task<IEnumerable<CategoryDto>> GetCategoriesAsync(Guid mainCategoryId);
        //Task<IEnumerable<ProductDto>> GetProductsAsync(Guid mainCategoryId);
    }
}
