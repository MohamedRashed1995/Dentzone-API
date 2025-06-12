using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetByIdWithItemsAsync(Guid id);
        Task<List<Guid>> GetCompletedOrderIdsAsync();
    }
}
