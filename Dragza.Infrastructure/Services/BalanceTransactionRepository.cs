using Dragza.Application.Data;
using Dragza.Application.Interface;
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

    public class BalanceTransactionRepository : Repository<BalanceTransaction>, IBalanceTransactionRepository
    {
        public BalanceTransactionRepository(DragzaContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BalanceTransaction>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.BalanceTransactions
                .Where(bt => bt.BalanceAccountId == accountId)
                .OrderByDescending(bt => bt.TransactionDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<BalanceTransaction>> GetByUserAsync(Guid userId)
        {
            return await _context.BalanceTransactions
                .Where(bt => bt.UserId == userId)
                .OrderByDescending(bt => bt.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BalanceTransaction>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.BalanceTransactions
                .Where(bt => bt.OrderId == orderId)
                .ToListAsync();
        }
    }
}
