using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class BalanceAccount
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int AccountType { get; set; }

    public decimal Balance { get; set; }

    public decimal CreditLimit { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BalanceTransaction> BalanceTransactions { get; set; } = new List<BalanceTransaction>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
