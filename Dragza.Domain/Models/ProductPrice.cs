using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dragza.Domain.Models;

public partial class ProductPrice
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    //public Guid? CategoryId { get; set; }

    public decimal? PurchasePrice { get; set; }

    public decimal? SalesPrice { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }

  

    public int StockQuantity { get; set; }
    public int MaxQuantity { get; set; }

    [Column("Discount Rate")]  // exact column name in SQL Server
    public decimal DiscountRate { get; set; }

    //public virtual Category? Category { get; set; } = null!;


    public Guid InventoryUserId { get; set; }
   [ForeignKey("InventoryUserId")]
    public virtual User InventoryUser { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ReturnedItem> ReturnedItems { get; set; } = new List<ReturnedItem>();
}
