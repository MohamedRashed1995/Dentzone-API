using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class ProductPrice
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid CategoryId { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SalesPrice { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public Guid InventoryUserId { get; set; }

    public int StockQuantity { get; set; }

   // public Guid? MainCategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User InventoryUser { get; set; } = null!;

    //public virtual MainCategory? MainCategory { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ReturnedItem> ReturnedItems { get; set; } = new List<ReturnedItem>();
}
