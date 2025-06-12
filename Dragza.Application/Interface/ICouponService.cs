using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface ICouponService
    {
        Task<decimal> ApplyCouponAsync(string couponCode, decimal amount, Guid userId);
        Task<IEnumerable<CouponDto>> GetAvailableCouponsAsync(Guid userId, decimal orderTotal = 0);
        Task RemoveCouponAsync(Guid orderId, Guid userId);
        Task<bool> ValidateCouponAsync(string couponCode, Guid userId, decimal amount);

        Task<CouponDto> CreateCouponAsync(CreateCouponDto dto);
        Task<CouponDto> GetCouponByIdAsync(Guid id);
        Task<IEnumerable<CouponDto>> GetAllCouponsAsync(bool activeOnly = false);
        Task<CouponDto> UpdateCouponAsync(Guid id, UpdateCouponDto dto);
        Task DeleteCouponAsync(Guid id);

        // Coupon Usage Tracking
        Task<IEnumerable<CouponUsageDto>> GetCouponUsagesAsync(Guid couponId);
        Task<int> GetRedemptionCountAsync(Guid couponId);


    }
}
