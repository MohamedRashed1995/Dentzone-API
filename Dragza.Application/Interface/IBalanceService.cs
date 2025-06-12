using Dragza.Domain.DTO;
using Dragza.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IBalanceService
    {
        Task<UserBalancesDto> GetUserBalances(Guid userId);
        Task<BalanceAccountDto> GetBalanceAccount(Guid accountId);
        Task<BalanceTransactionDto> CreateTransaction(Guid accountId, decimal amount, Guid userId,
            TransactionType type, string description, Guid? orderId = null);
        Task<bool> HasSufficientBalance(Guid accountId, decimal amount);
        Task<IEnumerable<BalanceTransactionDto>> GetAccountTransactions(Guid accountId);
        Task<IEnumerable<BalanceTransactionDto>> GetUserTransactions(Guid userId);
        Task<BalanceAccountDto> UpdateCreditLimit(Guid accountId, decimal newLimit);
    }
}
