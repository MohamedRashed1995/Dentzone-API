using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface ICouponUsageRepository : IRepository<CouponUsage> 
    {
        Task<int> CountAsync(Expression<Func<CouponUsage, bool>> predicate);

    }

}
