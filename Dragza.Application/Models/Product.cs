using System;
using System.Collections.Generic;

namespace Dragza.Application.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Preef { get; set; }

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public Guid? ActiveIngerdientId { get; set; }

    public Guid? MainCategoryId { get; set; }

    public string? Image { get; set; }

    public string? ArabicName { get; set; }

    public virtual ICollection<BestSellerProduct> BestSellerProducts { get; set; } = new List<BestSellerProduct>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

    public virtual ICollection<ReturnedItem> ReturnedItems { get; set; } = new List<ReturnedItem>();
}
