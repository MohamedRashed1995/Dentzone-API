using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
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

    public class BalanceReportingRepository : IBalanceReportingRepository
    {
        private readonly DragzaContext _context;

        public BalanceReportingRepository(DragzaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BalanceAccount>> GetBalanceAccountsReportAsync(BalanceReportFilterDto filter)
        {
            var query = _context.BalanceAccounts
                .Include(ba => ba.User)
                .Include(ba => ba.BalanceTransactions)
                .AsQueryable();

            query = ApplyBalanceAccountFilters(query, filter);

            return await query
                .OrderByDescending(ba => ba.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<BalanceTransaction>> GetBalanceTransactionsReportAsync(BalanceReportFilterDto filter)
        {
            var query = _context.BalanceTransactions
                .Include(bt => bt.BalanceAccount)
                    .ThenInclude(ba => ba.User)
                .Include(bt => bt.Order)
                .AsQueryable();

            query = ApplyBalanceTransactionFilters(query, filter);

            return await query
                .OrderByDescending(bt => bt.TransactionDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<BalanceSummaryDto> GetBalanceSummaryAsync(BalanceReportFilterDto filter)
        {
            var accountQuery = _context.BalanceAccounts.AsQueryable();
            var transactionQuery = _context.BalanceTransactions.AsQueryable();

            accountQuery = ApplyBalanceAccountFilters(accountQuery, filter);
            transactionQuery = ApplyBalanceTransactionFilters(transactionQuery, filter);

            var totalAccounts = await accountQuery.CountAsync();
            var totalTransactions = await transactionQuery.CountAsync();
            var totalBalance = await accountQuery.SumAsync(ba => ba.Balance);
            var totalCreditLimit = await accountQuery.SumAsync(ba => ba.CreditLimit);
            var totalDeposits = await transactionQuery
                .Where(t => t.Amount > 0)
                .SumAsync(t => t.Amount);
            var totalWithdrawals = await transactionQuery
                .Where(t => t.Amount < 0)
                .SumAsync(t => Math.Abs(t.Amount));

            return new BalanceSummaryDto
            {
                TotalAccounts = totalAccounts,
                TotalTransactions = totalTransactions,
                TotalBalance = totalBalance,
                TotalCreditLimit = totalCreditLimit,
                TotalDeposits = totalDeposits,
                TotalWithdrawals = totalWithdrawals
            };
        }

        public async Task<int> GetBalanceAccountsCountAsync(BalanceReportFilterDto filter)
        {
            var query = _context.BalanceAccounts.AsQueryable();
            query = ApplyBalanceAccountFilters(query, filter);
            return await query.CountAsync();
        }

        public async Task<int> GetBalanceTransactionsCountAsync(BalanceReportFilterDto filter)
        {
            var query = _context.BalanceTransactions.AsQueryable();
            query = ApplyBalanceTransactionFilters(query, filter);
            return await query.CountAsync();
        }

        private IQueryable<BalanceAccount> ApplyBalanceAccountFilters(IQueryable<BalanceAccount> query, BalanceReportFilterDto filter)
        {
            if (filter.UserId.HasValue)
            {
                query = query.Where(ba => ba.UserId == filter.UserId.Value);
            }

            if (filter.AccountType.HasValue)
            {
                query = query.Where(ba => ba.AccountType == filter.AccountType.Value);
            }

            return query;
        }

        private IQueryable<BalanceTransaction> ApplyBalanceTransactionFilters(IQueryable<BalanceTransaction> query, BalanceReportFilterDto filter)
        {
            if (filter.StartDate.HasValue)
            {
                query = query.Where(bt => bt.TransactionDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(bt => bt.TransactionDate <= filter.EndDate.Value);
            }

            if (filter.UserId.HasValue)
            {
                query = query.Where(bt => bt.BalanceAccount.UserId == filter.UserId.Value);
            }

            if (filter.AccountType.HasValue)
            {
                query = query.Where(bt => bt.BalanceAccount.AccountType == filter.AccountType.Value);
            }

            if (filter.TransactionType.HasValue)
            {
                query = query.Where(bt => bt.TransactionType == filter.TransactionType.Value);
            }

            if (filter.OrderId.HasValue)
            {
                query = query.Where(bt => bt.OrderId == filter.OrderId.Value);
            }

            return query;
        }
    }
}
