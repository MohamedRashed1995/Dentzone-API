using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class ActiveIngredient
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
