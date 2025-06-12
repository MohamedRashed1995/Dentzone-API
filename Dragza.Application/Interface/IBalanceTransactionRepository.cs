using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IBalanceTransactionRepository : IRepository<BalanceTransaction>
    {
        Task<IEnumerable<BalanceTransaction>> GetByAccountIdAsync(Guid accountId);
        Task<IEnumerable<BalanceTransaction>> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<BalanceTransaction>> GetByUserAsync(Guid userId);
    }
}
