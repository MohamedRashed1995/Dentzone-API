using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon> GetByCodeAsync(string code);
        Task<IEnumerable<Coupon>> GetActiveCouponsAsync();
        Task<int> GetUsageCountAsync(Guid couponId);
        Task<int> GetUserUsageCountAsync(Guid couponId, Guid userId);
    }
}
