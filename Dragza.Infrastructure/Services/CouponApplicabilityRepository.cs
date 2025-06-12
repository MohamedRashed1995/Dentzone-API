using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{

    public class CouponApplicabilityRepository : Repository<CouponApplicability>, ICouponApplicabilityRepository
    {
        public CouponApplicabilityRepository(DragzaContext context) : base(context) { }
    }
}
