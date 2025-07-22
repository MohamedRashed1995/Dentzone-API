using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class ActiveIngredient
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
