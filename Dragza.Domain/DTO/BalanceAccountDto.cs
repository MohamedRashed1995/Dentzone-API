using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class BalanceAccountDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string AccountType { get; set; } // "Cash" or "Credit"
        public decimal Balance { get; set; }
        public decimal CreditLimit { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
