using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{

    public class BalanceService : IBalanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BalanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserBalancesDto> GetUserBalances(Guid userId)
        {
            var cashAccount = await GetOrCreateAccount(userId, BalanceAccountType.Cash);
            var creditAccount = await GetOrCreateAccount(userId, BalanceAccountType.Credit);

            return new UserBalancesDto
            {
                CashAccount = _mapper.Map<BalanceAccountDto>(cashAccount),
                CreditAccount = _mapper.Map<BalanceAccountDto>(creditAccount)
            };
        }

        private async Task<BalanceAccount> GetOrCreateAccount(Guid userId, BalanceAccountType accountType)
        {
            var account = await _unitOfWork.BalanceAccountRepository.GetByUserAndTypeAsync(userId, accountType);
            if (account == null)
            {
                account = new BalanceAccount
                {
                    Id = new Guid(),
                    UserId = userId,
                    AccountType = (int)accountType,
                    Balance = 0,
                    CreditLimit = accountType == BalanceAccountType.Credit ? 10000 : 0 // Default credit limit
                };
                await _unitOfWork.BalanceAccountRepository.AddAsync(account);
                await _unitOfWork.CommitAsync();
            }
            return account;
        }

        public async Task<BalanceAccountDto> GetBalanceAccount(Guid accountId)
        {
            var account = await _unitOfWork.BalanceAccountRepository.GetByIdAsync(accountId);
            if (account == null) throw new KeyNotFoundException("Account not found");
            return _mapper.Map<BalanceAccountDto>(account);
        }

        public async Task<BalanceTransactionDto> CreateTransaction(Guid accountId, decimal amount,Guid userId,
            TransactionType type, string description, Guid? orderId = null)
        {
            var account = await _unitOfWork.BalanceAccountRepository.GetByIdAsync(accountId);
            if (account == null) throw new KeyNotFoundException("Account not found");

            // Validate credit limit for credit accounts
            if (account.AccountType == (int)BalanceAccountType.Credit &&
                type == TransactionType.Withdrawal &&
                (account.Balance - amount) < -account.CreditLimit)
            {
                throw new InvalidOperationException("Transaction would exceed credit limit");
            }

            var transaction = new BalanceTransaction
            {
                BalanceAccountId = accountId,
                Amount = amount,
                TransactionType = (int)type,
                Description = description,
                OrderId = orderId,
                TransactionDate = DateTime.UtcNow,
                UserId = userId
            };

            // Update account balance
            account.Balance += amount;
            account.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.BalanceTransactionRepository.AddAsync(transaction);
            _unitOfWork.BalanceAccountRepository.Update(account);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<BalanceTransactionDto>(transaction);
        }

        public async Task<bool> HasSufficientBalance(Guid accountId, decimal amount)
        {
            var account = await _unitOfWork.BalanceAccountRepository.GetByIdAsync(accountId);
            if (account == null) return false;

            // For credit accounts, check against credit limit
            if (account.AccountType == (int)BalanceAccountType.Credit)
            {
                return (account.Balance - amount) >= -account.CreditLimit;
            }

            // For cash accounts, simple balance check
            return account.Balance >= amount;
        }

        public async Task<IEnumerable<BalanceTransactionDto>> GetAccountTransactions(Guid accountId)
        {
            var transactions = await _unitOfWork.BalanceTransactionRepository.GetByAccountIdAsync(accountId);
            return _mapper.Map<IEnumerable<BalanceTransactionDto>>(transactions);
        }
        public async Task<IEnumerable<BalanceTransactionDto>> GetUserTransactions(Guid userId)
        {
            var transactions = await _unitOfWork.BalanceTransactionRepository.GetByUserAsync(userId);
            return _mapper.Map<IEnumerable<BalanceTransactionDto>>(transactions);
        }

        public async Task<BalanceAccountDto> UpdateCreditLimit(Guid accountId, decimal newLimit)
        {
            var account = await _unitOfWork.BalanceAccountRepository.GetByIdAsync(accountId);
            if (account == null) throw new KeyNotFoundException("Account not found");
            if (account.AccountType != (int)BalanceAccountType.Credit)
                throw new InvalidOperationException("Only credit accounts have limits");

            account.CreditLimit = newLimit;
            account.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.BalanceAccountRepository.Update(account);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<BalanceAccountDto>(account);
        }
    }
}
