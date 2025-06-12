using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class Coupon
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscountAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? UsageLimit { get; set; }

    public int? PerUserLimit { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CouponApplicability> CouponApplicabilities { get; set; } = new List<CouponApplicability>();

    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
}
