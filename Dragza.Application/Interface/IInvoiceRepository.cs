using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<Invoice> GetByIdAsync(Guid id);
        Task<Invoice> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Invoice>> GetByInvintoryIdAsync(Guid userId);
        Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Invoice> AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsForOrderAsync(Guid orderId);
    }
}
