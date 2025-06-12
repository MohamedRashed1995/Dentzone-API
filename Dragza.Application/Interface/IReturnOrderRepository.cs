using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IReturnOrderRepository : IRepository<ReturnOrder>
    {
        Task<ReturnOrder> GetWithItemsAsync(Guid id);
        Task<IEnumerable<ReturnOrder>> GetByPharmacyAsync(Guid pharmacyId);
        Task<IEnumerable<ReturnOrder>> GetByInventoryUserAsync(Guid inventoryUserId);
    }
}
