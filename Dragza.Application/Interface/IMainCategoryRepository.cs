using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IMainCategoryRepository : IRepository<MainCategory>
    {
        Task<IEnumerable<Category>> GetCategoriesByMainCategoryAsync(Guid mainCategoryId);
        Task<IEnumerable<Product>> GetProductsByMainCategoryAsync(Guid mainCategoryId);
    }
}
