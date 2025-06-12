using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class InvoiceType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
