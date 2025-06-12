using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class Region
{
    public Guid Id { get; set; }

    public string RegionName { get; set; } = null!;

    public string? Lang { get; set; }

    public string? Lat { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Governate> Governates { get; set; } = new List<Governate>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
