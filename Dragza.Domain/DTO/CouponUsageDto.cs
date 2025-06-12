using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CouponUsageDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public Guid OrderId { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime UsedAt { get; set; }
    }
}
