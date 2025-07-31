using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IProductPriceRepository : IRepository<ProductPrice>
    {
        Task<IEnumerable<ProductPrice>> GetPricesByProductAsync(Guid productId);
        Task<IEnumerable<ProductPrice>> GetPricesByInventoryUserAsync(Guid userId);
        Task<List<ProductPrice>> GetBestPricesAsync();
        Task<List<ProductPrice>> GetPricesWithDetailsByCategory(Guid categoryId);
        Task<List<ProductPrice>> GetPricesByProduct(Guid productId);
        Task<List<ProductPrice>> GetPricesByInventoryUser(Guid userId);
        Task<IEnumerable<ProductPrice>> GetAllProductPricesWithDetailsAsync(bool includeDeleted, string search);

		Task<ProductPrice> GetPricesByInventoryAndProduct(Guid userId, Guid productId);
    }
}
