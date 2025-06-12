using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class CouponApplicabilityDto
    {
        public string ApplicableType { get; set; }
        public Guid? ApplicableId { get; set; }
    }
}
