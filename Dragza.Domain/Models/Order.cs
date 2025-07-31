using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid PharmacyUserId { get; set; }

    public DateTime OrderDate { get; set; }

    public int Status { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public Guid? InventoryUserId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? DeliverDate { get; set; }

    public Guid? CouponId { get; set; }

    public decimal? CreditUsed { get; set; }

    public decimal? CashPaid { get; set; }

    public Guid? CreditAccountId { get; set; }

    public string? OrderNumber { get; set; }

    public virtual ICollection<BalanceTransaction> BalanceTransactions { get; set; } = new List<BalanceTransaction>();

    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();

    public virtual BalanceAccount? CreditAccount { get; set; }

    public virtual User? InventoryUser { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User PharmacyUser { get; set; } = null!;

    public virtual ICollection<ReturnOrder> ReturnOrders { get; set; } = new List<ReturnOrder>();

    public virtual ICollection<ReturnedItem> ReturnedItems { get; set; } = new List<ReturnedItem>();
}
