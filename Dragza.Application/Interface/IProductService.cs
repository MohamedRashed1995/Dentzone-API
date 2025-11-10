using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductResponseDto> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(bool includeDeleted, string search, int page = 1, int size = 10);
        Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductDto dto);
        Task SoftDeleteProductAsync(Guid id);
        Task RestoreProductAsync(Guid id);
        Task<IEnumerable<ProductResponseDto>> GetProductsByActiveIngredientAsync(Guid activeIngredientId);
        Task<IEnumerable<BestSellerProductDto>> GetBestSellingProductsAsync(int topN = 10);
        Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(Guid categoryId);

        Task<List<ProductPrice>> GetPricesWithAllProductByInventoryId(Guid inventoryId);

    }
}
