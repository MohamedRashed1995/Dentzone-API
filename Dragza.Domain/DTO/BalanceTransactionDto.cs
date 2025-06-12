using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class BalanceTransactionDto
    {
        public Guid Id { get; set; }
        public Guid BalanceAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string TransactionType { get; set; }
        public Guid? OrderId { get; set; }
    }
}
