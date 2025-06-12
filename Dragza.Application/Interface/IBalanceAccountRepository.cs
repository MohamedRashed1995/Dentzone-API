using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IBalanceAccountRepository : IRepository<BalanceAccount>
    {
        Task<BalanceAccount> GetByUserAndTypeAsync(Guid userId, BalanceAccountType accountType);
    }
}
