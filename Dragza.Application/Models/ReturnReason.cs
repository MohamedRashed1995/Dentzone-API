using System;
using System.Collections.Generic;

namespace Dragza.Application.Models;

public partial class ReturnReason
{
    public Guid Id { get; set; }

    public string Reason { get; set; } = null!;

    public virtual ICollection<ReturnedItem> ReturnedItems { get; set; } = new List<ReturnedItem>();
}
