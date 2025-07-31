using System;
using System.Collections.Generic;

namespace Dragza.Application.Models;

public partial class ReturnedItem
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid OrderId { get; set; }

    public Guid ReturnOrderId { get; set; }

    public int QuantityReturned { get; set; }

    public Guid ReasonId { get; set; }

    public string? OtherReason { get; set; }

    public Guid ProductPriceId { get; set; }

    public decimal TotalAmount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ProductPrice ProductPrice { get; set; } = null!;

    public virtual ReturnReason Reason { get; set; } = null!;

    public virtual ReturnOrder ReturnOrder { get; set; } = null!;
}
