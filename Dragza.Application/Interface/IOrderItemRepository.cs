using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        IQueryable<BestSellerProduct> GetBestSellingProductsQuery(List<Guid> orderIds);
        Task<List<BestSellerProduct>> GetBestSellingProductsAsync(int topN);



    }
}
