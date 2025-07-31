using System;
using System.Collections.Generic;

namespace Dragza.Application.Models;

public partial class Destrict
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid CityId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual City City { get; set; } = null!;
}
