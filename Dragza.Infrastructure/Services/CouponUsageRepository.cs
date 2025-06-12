using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class CouponUsageRepository : Repository<CouponUsage>, ICouponUsageRepository
    {
        public CouponUsageRepository(DragzaContext context) : base(context) { }

        public async Task<int> CountAsync(Expression<Func<CouponUsage, bool>> predicate)
        {
            return await _context.Set<CouponUsage>().Where(predicate).CountAsync();
        }
    }
}
