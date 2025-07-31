using System;
using System.Collections.Generic;

namespace Dragza.Application.Models;

public partial class ActiveIngredient
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public bool IsMeltable { get; set; }
}
