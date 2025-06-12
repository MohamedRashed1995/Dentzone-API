// Application/Services/CouponService.cs
using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace Dragza.Infrastructure.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public CouponService(IUnitOfWork unitOfWork, IMapper mapper, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderService = orderService;
        }

        public async Task<decimal> ApplyCouponAsync(string couponCode, decimal amount, Guid userId)
        {
            
            var coupon = await PrivateValidateCouponAsync(couponCode, userId, amount);
            var discountAmount = CalculateDiscount(coupon, amount);


            return discountAmount;
        }

        public async Task<IEnumerable<CouponDto>> GetAvailableCouponsAsync(Guid userId, decimal orderTotal = 0)
        {
            var now = DateTime.UtcNow;
            var coupons = await _unitOfWork.CouponRepository.GetActiveCouponsAsync();

            var availableCoupons = new List<CouponDto>();

            foreach (var coupon in coupons)
            {
                if (!await IsCouponApplicableToUserAsync(coupon, userId))
                    continue;

                var usageCount = await _unitOfWork.CouponRepository.GetUsageCountAsync(coupon.Id);
                if (coupon.UsageLimit.HasValue && usageCount >= coupon.UsageLimit)
                    continue;

                var userUsageCount = await _unitOfWork.CouponRepository.GetUserUsageCountAsync(coupon.Id, userId);
                if (userUsageCount >= coupon.PerUserLimit)
                    continue;

                if (orderTotal < coupon.MinimumOrderAmount)
                    continue;

                availableCoupons.Add(_mapper.Map<CouponDto>(coupon));
            }

            return availableCoupons;
        }

        private async Task<Coupon> PrivateValidateCouponAsync(string couponCode, Guid userId, decimal amount)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByCodeAsync(couponCode);
            if (coupon == null || coupon.IsActive == false)
                throw new Exception("Invalid coupon code");

            var now = DateTime.UtcNow;

            if (now < coupon.StartDate)
                throw new Exception("Coupon is not yet valid");

            if (now > coupon.EndDate)
                throw new Exception("Coupon has expired");

            var usageCount = await _unitOfWork.CouponRepository.GetUsageCountAsync(coupon.Id);
            if (coupon.UsageLimit.HasValue && usageCount >= coupon.UsageLimit)
                throw new Exception("Coupon usage limit reached");

            var userUsageCount = await _unitOfWork.CouponRepository.GetUserUsageCountAsync(coupon.Id, userId);
            if (userUsageCount >= coupon.PerUserLimit)
                throw new Exception("You have reached the maximum usage limit for this coupon");

            if (amount < coupon.MinimumOrderAmount)
                throw new Exception($"Minimum order amount of {coupon.MinimumOrderAmount} required");

            if (!await IsCouponApplicableToUserAsync(coupon, userId))
                throw new Exception("This coupon is not applicable to you");

            return coupon;
        }

        private async Task<bool> IsCouponApplicableToUserAsync(Coupon coupon, Guid userId)
        {
            var applicabilities = await _unitOfWork.CouponApplicabilityRepository
                .FindAsync(ca => ca.CouponId == coupon.Id);

            if (!applicabilities.Any())
                return true;

            return applicabilities.Any(a =>
                a.ApplicableType == "all" ||
                (a.ApplicableType == "user" && a.ApplicableId == userId));
        }

        private async Task<bool> IsCouponApplicableToOrderAsync(Coupon coupon, Order order)
        {
            var applicabilities = await _unitOfWork.CouponApplicabilityRepository
                .FindAsync(ca => ca.CouponId == coupon.Id);

            if (!applicabilities.Any())
                return true;

            var orderItems = await _unitOfWork.OrderItemRepository
                .FindAsync(oi => oi.OrderId == order.Id);

            if (!orderItems.Any())
                return false;

            var productIds = orderItems.Select(oi => oi.ProductId).ToList();
            var categoryIds = orderItems
                .Select(oi => oi.Product.CategoryId)
                .ToList();

            return applicabilities.Any(a =>
                a.ApplicableType == "all" ||
                (a.ApplicableType == "product" && productIds.Contains(a.ApplicableId.Value)) ||
                (a.ApplicableType == "category" && categoryIds.Contains(a.ApplicableId.Value)));
        }

        private decimal CalculateDiscount(Coupon coupon, decimal amount)
        {
            if (coupon.DiscountType == "percentage")
            {
                var discount = amount * (coupon.DiscountValue / 100);
                if (coupon.MaximumDiscountAmount.HasValue)
                {
                    discount = Math.Min(discount, coupon.MaximumDiscountAmount.Value);
                }
                return discount;
            }
            return Math.Min(coupon.DiscountValue, amount);
        }

        public async Task RemoveCouponAsync(Guid orderId, Guid userId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (!order.CouponId.HasValue)
                throw new Exception("No coupon applied to this order");

            // Get coupon usage record
            var couponUsage = await _unitOfWork.CouponUsageRepository
                .FindAsync(cu => cu.OrderId == orderId && cu.CouponId == order.CouponId.Value);

            if (!couponUsage.Any())
                throw new Exception("Coupon usage record not found");

            // Restore original amount
            order.TotalAmount += couponUsage.First().DiscountAmount;
            order.CouponId = null;

            // Remove coupon usage record
            _unitOfWork.CouponUsageRepository.Delete(couponUsage.First());
            _unitOfWork.OrderRepository.Update(order);

            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> ValidateCouponAsync(string couponCode, Guid userId, decimal amount)
        {

            var coupon = await PrivateValidateCouponAsync(couponCode, userId, amount);
            if (coupon == null)
            {
                return false;
            }
            else
                return true;
        }

        // Add these methods to your existing CouponService implementation

        public async Task<CouponDto> CreateCouponAsync(CreateCouponDto dto)
        {
            // Validate dates
            if (dto.StartDate >= dto.EndDate)
                throw new Exception("End date must be after start date");

            // Check if code exists
            var existing = await _unitOfWork.CouponRepository.GetByCodeAsync(dto.Code);
            if (existing != null)
                throw new Exception("Coupon code already exists");

            var coupon = _mapper.Map<Coupon>(dto);
            await _unitOfWork.CouponRepository.AddAsync(coupon);
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<CouponDto> GetCouponByIdAsync(Guid id)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            if (coupon == null)
                throw new Exception("Coupon not found");

            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<IEnumerable<CouponDto>> GetAllCouponsAsync(bool activeOnly = false)
        {
            IEnumerable<Coupon> coupons;

            if (activeOnly)
            {
                var now = DateTime.UtcNow;
                coupons = await _unitOfWork.CouponRepository.FindAsync(
                    c => c.IsActive == true && c.StartDate <= now && c.EndDate >= now);
            }
            else
            {
                coupons = await _unitOfWork.CouponRepository.GetAllAsync();
            }

            return _mapper.Map<IEnumerable<CouponDto>>(coupons);
        }

        public async Task<IEnumerable<CouponUsageDto>> GetCouponUsagesAsync(Guid couponId)
        {
            var usages = await _unitOfWork.CouponUsageRepository.FindAsync(
                cu => cu.CouponId == couponId);

            return usages.Select(u => new CouponUsageDto
            {
                Id = u.Id,
                UserId = u.UserId,
                UserEmail = u.User.Email,
                OrderId = u.OrderId,
                OrderTotal = u.Order.TotalAmount + u.DiscountAmount, // Original amount
                DiscountAmount = u.DiscountAmount,
                UsedAt = (DateTime)u.UsedAt
            });
        }

        public async Task<int> GetRedemptionCountAsync(Guid couponId)
        {
            return await _unitOfWork.CouponUsageRepository.CountAsync(cu => cu.CouponId == couponId);
        }

        public async Task DeleteCouponAsync(Guid id)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            if (coupon == null)
                throw new Exception("Coupon not found");

            // Check if coupon has been used
            var usageCount = await _unitOfWork.CouponRepository.GetUsageCountAsync(id);
            if (usageCount > 0)
                throw new Exception("Cannot delete coupon that has been used");

            _unitOfWork.CouponRepository.Delete(coupon);
            await _unitOfWork.CommitAsync();
        }

        public async Task<CouponDto> UpdateCouponAsync(Guid id, UpdateCouponDto dto)
        {
            var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(id);
            if (coupon == null)
                throw new Exception("Coupon not found");

            // Only update allowed fields
            coupon.Description = dto.Description;
            coupon.StartDate = dto.StartDate;
            coupon.EndDate = dto.EndDate;
            coupon.UsageLimit = dto.UsageLimit;
            coupon.IsActive = dto.IsActive;
            coupon.UpdatedAt = DateTime.UtcNow;

            // Validate dates
            if (coupon.StartDate >= coupon.EndDate)
                throw new Exception("End date must be after start date");

            _unitOfWork.CouponRepository.Update(coupon);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<CouponDto>(coupon);
        }
    }
}