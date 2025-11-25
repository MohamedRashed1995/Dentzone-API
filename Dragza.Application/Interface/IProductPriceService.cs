using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IProductPriceService
    {
        Task<ProductPriceResponseDto> CreateProductPriceAsync(CreateProductPriceDto dto, Guid userId);
        Task<IEnumerable<ProductPriceResponseDto>> GetPricesByUserAsync(Guid userId);
        Task<IEnumerable<ProductBestPriceDto>> GetProductsBestPricesAsync(int page = 1, int size = 10);
        Task<IEnumerable<ProductPriceDetailsDto>> GetPricesByCategoryAsync(Guid categoryId);
        Task<IEnumerable<ProductBestPriceDto>> GetProductByCategoryAsync(Guid categoryId);
        Task<IEnumerable<ProductPriceDetailsDto>> GetPricesByProductAsync(Guid productId);
		Task<IEnumerable<ProductPriceResponseDto>> GetAllProductPricesAsync(bool includeDeleted, string search, int page = 1, int size = 10);
		Task<IEnumerable<ProductPriceResponseDto>> GetAllProductAsync(Guid productId);
        Task<IEnumerable<InventoryUserPriceDetailsDto>> GetPricesByInventoryUserAsync(Guid userId);
        Task UpdateProductPriceAndQuantityAsync(Guid productId, Guid productPriceId, decimal salesPrice, decimal purchasePrice, int quantity ,int maxQuantity, Guid userId);
        Task<IEnumerable<ProductBestPriceDto>> GetProductsBestPricesSortingAsync(int sort, int page = 1, int size = 10);

    }
}
