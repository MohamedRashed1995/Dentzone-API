using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class Governate
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? RegionId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Region? Region { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
