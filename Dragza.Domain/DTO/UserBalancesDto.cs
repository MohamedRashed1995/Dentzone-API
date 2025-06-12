using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UserBalancesDto
    {
        public BalanceAccountDto CashAccount { get; set; }
        public BalanceAccountDto CreditAccount { get; set; }
    }
}
