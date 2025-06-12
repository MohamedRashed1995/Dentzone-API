using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        public CouponRepository(DragzaContext context) : base(context) { }

        public async Task<Coupon> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .Include(c => c.CouponApplicabilities)
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Coupons
                .Include(c => c.CouponApplicabilities)
                .Where(c => c.IsActive == true && c.StartDate <= now && c.EndDate >= now)
                .ToListAsync();
        }

        public async Task<int> GetUsageCountAsync(Guid couponId)
        {
            return await _context.CouponUsages
                .CountAsync(cu => cu.CouponId == couponId);
        }

        public async Task<int> GetUserUsageCountAsync(Guid couponId, Guid userId)
        {
            return await _context.CouponUsages
                .CountAsync(cu => cu.CouponId == couponId && cu.UserId == userId);
        }
    }
}
