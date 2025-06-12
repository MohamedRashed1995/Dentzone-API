using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class City
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid GovernateId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Destrict> Destricts { get; set; } = new List<Destrict>();

    public virtual Governate Governate { get; set; } = null!;
}
