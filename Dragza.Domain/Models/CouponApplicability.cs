using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class CouponApplicability
{
    public Guid Id { get; set; }

    public Guid CouponId { get; set; }

    public string ApplicableType { get; set; } = null!;

    public Guid? ApplicableId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Coupon Coupon { get; set; } = null!;
}
