using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Preef { get; set; }
    public string? ArabicPreef { get; set; }
    public string? Description { get; set; }
    public string? ArabicDescription { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Image { get; set; }
    public string? ArabicName { get; set; }
    public virtual ICollection<BestSellerProduct> BestSellerProducts { get; set; } = new List<BestSellerProduct>();
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();
    public virtual ICollection<ReturnedItem> ReturnedItems { get; set; } = new List<ReturnedItem>();
    public int? ProductCode { get; set; }
}
