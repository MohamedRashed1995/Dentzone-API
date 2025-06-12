using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class BalanceAccountRepository : Repository<BalanceAccount>, IBalanceAccountRepository
    {
        public BalanceAccountRepository(DragzaContext context) : base(context)
        {
        }

        public async Task<BalanceAccount> GetByUserAndTypeAsync(Guid userId, BalanceAccountType accountType)
        {
            return await _context.BalanceAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId && ba.AccountType == (int)accountType);
        }
    }
}
