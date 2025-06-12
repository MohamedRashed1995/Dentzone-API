using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class BalanceTransaction
{
    public Guid Id { get; set; }

    public Guid BalanceAccountId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string Description { get; set; } = null!;

    public int TransactionType { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? RelatedTransactionId { get; set; }

    public Guid? UserId { get; set; }

    public virtual BalanceAccount BalanceAccount { get; set; } = null!;

    public virtual ICollection<BalanceTransaction> InverseRelatedTransaction { get; set; } = new List<BalanceTransaction>();

    public virtual Order? Order { get; set; }

    public virtual BalanceTransaction? RelatedTransaction { get; set; }
}
