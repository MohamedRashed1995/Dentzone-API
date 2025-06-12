using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class BestSellerProduct
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public int? TotalQuantitySold { get; set; }

    public decimal? TotalRevenue { get; set; }

    public virtual Product Product { get; set; } = null!;
}
