using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CreateCouponDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; } // "percentage" or "fixed_amount"
        public decimal DiscountValue { get; set; }
        public decimal MinimumOrderAmount { get; set; } = 0;
        public decimal? MaximumDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? UsageLimit { get; set; }
        public int PerUserLimit { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public List<CouponApplicabilityDto>? Applicabilities { get; set; }
    }
}
