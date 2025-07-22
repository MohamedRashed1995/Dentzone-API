using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class BalanceAccountReportDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string AccountType { get; set; } // "Cash" or "Credit"
        public decimal Balance { get; set; }
        public decimal CreditLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
    }

    public class BalanceTransactionReportDto
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public Guid? OrderId { get; set; }
        public string OrderNumber { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }

    public class BalanceSummaryDto
    {
        public decimal TotalBalance { get; set; }
        public decimal TotalCreditLimit { get; set; }
        public int TotalAccounts { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
    }

    public class BalanceReportFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? UserId { get; set; }
        public int? AccountType { get; set; } // 0 = Cash, 1 = Credit
        public int? TransactionType { get; set; }
        public Guid? OrderId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
