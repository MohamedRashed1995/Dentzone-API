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
        Task<IEnumerable<ProductBestPriceDto>> GetProductsBestPricesAsync();
        Task<IEnumerable<ProductPriceDetailsDto>> GetPricesByCategoryAsync(Guid categoryId);
        Task<IEnumerable<ProductPriceDetailsDto>> GetPricesByProductAsync(Guid productId);

        Task<IEnumerable<InventoryUserPriceDetailsDto>> GetPricesByInventoryUserAsync(Guid userId);

    }
}
