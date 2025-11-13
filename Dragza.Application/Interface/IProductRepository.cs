using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetProductWithDetailsAsync(Guid id);
        Task<List<Product>> GetAllProductsWithDetailsAsync(bool includeDeleted, string search, int page = 1, int size = 10);
        Task<List<Product>> GetByActiveIngredientAsync(Guid activeIngredientId);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
        Task<List<ProductPrice>> GetPricesWithDetailsByCategory(Guid categoryId);
        Task<List<Product>> GetPricesWithAllProductByInventoryId(Guid inventoryId);
        Task<Product> GetProductByName(string name);


    }
}
