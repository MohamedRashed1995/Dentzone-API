using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Pref { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public Guid? MainCategoryId { get; set; }

    public string? ArabicName { get; set; }

    public virtual MainCategory? MainCategory { get; set; }

    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
