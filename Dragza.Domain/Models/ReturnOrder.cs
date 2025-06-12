using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class ReturnOrder
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid PharmacyUserId { get; set; }

    public DateTime RequestDate { get; set; }

    public int Statuse { get; set; }

    public bool AdminApproval { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public decimal TotalReturnValue { get; set; }

    public Guid InventoryUserId { get; set; }

    public virtual User InventoryUser { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual User PharmacyUser { get; set; } = null!;

    public virtual ICollection<ReturnedItem> ReturnedItems { get; set; } = new List<ReturnedItem>();
}
