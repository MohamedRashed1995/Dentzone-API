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
        Task<List<Product>> GetAllProductsWithDetailsAsync(bool includeDeleted = false);
        Task<List<Product>> GetByActiveIngredientAsync(Guid activeIngredientId);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
        Task<List<ProductPrice>> GetPricesWithDetailsByCategory(Guid categoryId);


    }
}
