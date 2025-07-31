using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public Guid ProductPriceId { get; set; }

    public int Quantity { get; set; }

    public decimal Amount { get; set; }

    public int Status { get; set; }

    public Guid? InventoryId { get; set; }

    public virtual User? Inventory { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ProductPrice ProductPrice { get; set; } = null!;
}
