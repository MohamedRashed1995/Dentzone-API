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
        Task<IEnumerable<ProductPrice>> GetPricesByIdAsync(Guid productPriceId);
        Task<IEnumerable<ProductPrice>> GetPricesByInventoryUserAsync(Guid userId);
        Task<List<ProductPrice>> GetBestPricesAsync(int page=1,int size=10);
        Task<List<ProductPrice>> GetPricesWithDetailsByCategory(Guid categoryId);
        Task<List<ProductPrice>> GetProductByCategory(Guid categoryId);
        Task<List<ProductPrice>> GetPricesByProduct(Guid productId);
        Task<List<ProductPrice>> GetPricesByInventoryUser(Guid userId);
        Task<IEnumerable<ProductPrice>> GetAllProductPricesWithDetailsAsync(bool includeDeleted, string search, int page = 1, int size = 10);
        Task<IEnumerable<ProductPrice>> GetAllProductAsync(Guid productId);

        Task<ProductPrice> GetPricesByInventoryAndProduct(Guid userId, Guid productId);

        Task<List<ProductPrice>> GetBestPricesSortingAsync(int sort, int page = 1, int size = 10);
    }
}
