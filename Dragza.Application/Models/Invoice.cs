using System;
using System.Collections.Generic;

namespace Dragza.Application.Models;

public partial class Invoice
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public decimal TotalAmount { get; set; }

    public Guid PharmacyUserId { get; set; }

    public Guid InvoiceTypeId { get; set; }

    public virtual InvoiceType InvoiceType { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
