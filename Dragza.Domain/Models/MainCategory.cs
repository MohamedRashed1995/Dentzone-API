using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class MainCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
