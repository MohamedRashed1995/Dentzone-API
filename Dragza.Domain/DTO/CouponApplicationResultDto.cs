using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CouponApplicationResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NewTotal { get; set; }
        public CouponDto Coupon { get; set; }
    }
}
