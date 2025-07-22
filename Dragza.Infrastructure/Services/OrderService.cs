using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO.Order;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    // OrderService.cs
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBalanceService _balanceService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper , IBalanceService balanceService, IInvoiceService invoiceService , ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _balanceService = balanceService;
            _invoiceService = invoiceService;
            _logger = logger;
        }
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto , Guid pharmacyUserId)
        {
            try
            {

            _logger.LogInformation("CreateOrderAsync");
            // Validate user is a pharmacy
            var user = await _unitOfWork.UserRepository.GetByIdAsync(pharmacyUserId);
            if (user == null || user.IsPharmacy != true)
            {
                _logger.LogError("CreateOrderAsync :: Only pharmacy users can create orders");
                throw new InvalidOperationException("Only pharmacy users can create orders");
            }

            // Calculate total amount
            decimal totalAmount = orderDto.Items.Sum(item => item.Quantity * item.UnitPrice);

            // Apply coupon discount if any
            if (orderDto.CouponId.HasValue)
            {
                var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(orderDto.CouponId.Value);
                if (coupon != null && coupon.IsActive == true)
                {
                    totalAmount -= coupon.DiscountValue;
                }
            }

            // Handle payment based on payment method
            decimal cashAmount = 0;
            decimal creditAmount = 0;
            Guid? creditAccountId = null;

            switch (orderDto.PaymentMethod)
            {
                case PaymentMethod.Cash:
                    cashAmount = totalAmount;
                    break;

                case PaymentMethod.Credit:
                    creditAmount = totalAmount;
                    var creditAccount = (await _balanceService.GetUserBalances(pharmacyUserId)).CreditAccount;
                    //if (!await _balanceService.HasSufficientBalance(creditAccount.Id, -creditAmount))
                    //{
                    //    throw new InvalidOperationException("Insufficient credit balance");
                    //}
                    creditAccountId = creditAccount.Id;
                    break;

                case PaymentMethod.Mixed:
                    if (!orderDto.CreditAmount.HasValue)
                    {
                        throw new InvalidOperationException("Credit amount is required for mixed payment");
                    }
                    creditAmount = orderDto.CreditAmount.Value;
                    cashAmount = totalAmount - creditAmount;

                    var userCreditAccount = (await _balanceService.GetUserBalances(pharmacyUserId)).CreditAccount;
                    //if (!await _balanceService.HasSufficientBalance(userCreditAccount.Id, -creditAmount))
                    //{
                    //    throw new InvalidOperationException("Insufficient credit balance");
                    //}
                    creditAccountId = userCreditAccount.Id;
                    break;
            }

            // Create order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                PharmacyUserId = pharmacyUserId,
                OrderDate = DateTime.UtcNow,
                Status = (int)OrderStatus.Pending,
                TotalAmount = totalAmount,
                CreditUsed = creditAmount,
                CashPaid = cashAmount,
                CreditAccountId = creditAccountId,
            };

            // Add order items
            foreach (var itemDto in orderDto.Items)
            {
                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Id = Guid.NewGuid(),
                    Amount = itemDto.Quantity,
                    ProductPriceId = itemDto.ProductPriceId
                };
                order.OrderItems.Add(orderItem);
            }

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.CommitAsync();

            // Process payments
            if (creditAmount > 0)
            {
                await _balanceService.CreateTransaction(
                    creditAccountId.Value,
                    -creditAmount,
                    pharmacyUserId,
                    TransactionType.Payment,
                    $"Order payment #{order.Id}",
                    order.Id);
            }

            return _mapper.Map<OrderDto>(order);

            }
            catch (Exception ex)
            {
                _logger.LogError("CreateOrderAsync :: ", ex);
                _logger.LogError(ex.InnerException.Message);

                throw ex;
            }
        }
        //public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, Guid pharmacyUserId)
        //{
        //    using var transaction = await _unitOfWork.BeginTransactionAsync();

        //    try
        //    {
        //        var order = new Order
        //        {
        //            Id = Guid.NewGuid(),
        //            PharmacyUserId = pharmacyUserId,
        //            OrderDate = DateTime.UtcNow,
        //            Status = (int)OrderStatus.Pending
        //        };

        //        decimal totalAmount = 0;
        //        Guid? vendorId = null;

        //        foreach (var item in dto.Items)
        //        {
        //            var productPrice = await _unitOfWork.ProductPriceRepository.GetByIdAsync(item.ProductPriceId);
        //            if (productPrice == null)
        //                throw new KeyNotFoundException($"Product price not found: {item.ProductPriceId}");

        //            // Validate all items belong to same vendor
        //            if (vendorId.HasValue && productPrice.InventoryUserId != vendorId)
        //                throw new InvalidOperationException("All order items must belong to the same vendor");

        //            vendorId = productPrice.InventoryUserId;

        //            var orderItem = new OrderItem
        //            {
        //                Id = Guid.NewGuid(),
        //                OrderId = order.Id,
        //                ProductId = item.ProductId,
        //                ProductPriceId = item.ProductPriceId,
        //                Quantity = item.Quantity,
        //                Amount = item.Quantity * productPrice.SalesPrice
        //            };

        //            totalAmount += orderItem.Amount;
        //            await _unitOfWork.OrderItemRepository.AddAsync(orderItem);
        //        }

        //        order.TotalAmount = totalAmount;
        //        order.InventoryUserId = vendorId;
        //        await _unitOfWork.OrderRepository.AddAsync(order);

        //        await _unitOfWork.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        return _mapper.Map<OrderDto>(order);
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

        public async Task<OrderDto> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");
            var orderDetails = _mapper.Map<OrderDto>(order);
            if (order.CouponId != null)
            {
                orderDetails.CouponId = order.CouponId;
                orderDetails.DescountAmount = order.CouponUsages.FirstOrDefault().Coupon.DiscountValue;
                orderDetails.DescountType = order.CouponUsages.FirstOrDefault().Coupon.DiscountType;
            }
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(Guid userId)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                o => o.PharmacyUserId == userId,
                include: q => q.Include(o => o.OrderItems)
                              .ThenInclude(oi => oi.Product)
                              .Include(o => o.OrderItems)
                              .ThenInclude(oi => oi.ProductPrice)
            );
            return _mapper.Map<List<OrderDto>>(orders);
        }
        public async Task<List<OrderDto>> GetAllorders()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                                include: q => q.Include(o => o.OrderItems)
                              .ThenInclude(oi => oi.Product)
                              .Include(o => o.OrderItems)
                              .ThenInclude(oi => oi.ProductPrice)
            );
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<List<OrderDto>> GetVendorOrdersAsync(Guid vendorId)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                o => o.InventoryUserId == vendorId,
                include: q => q.Include(o => o.OrderItems)
                              .ThenInclude(oi => oi.Product)
                              .Include(u => u.InventoryUser)
                              .Include(u => u.PharmacyUser)
                              .Include(o => o.OrderItems)
                              .ThenInclude(oi => oi.ProductPrice)
            );
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task ReAssignOrder(ReAssignOrder reAssignOrderDto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(reAssignOrderDto.OrderId);
            order.InventoryUserId = reAssignOrderDto.UserId;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveItem(RemoveItemDto removeItemDto)
        {
            var item = await _unitOfWork.OrderItemRepository.GetByIdAsync(removeItemDto.ItemId);
            if (item.OrderId == removeItemDto.OrderId)
            {
                _unitOfWork.OrderItemRepository.Delete(item);
                _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus status, Guid userId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");

            // Authorization check
            if (order.InventoryUserId != userId)
                throw new UnauthorizedAccessException("Not authorized to modify this order");

            if (!IsValidStatusTransition((OrderStatus)order.Status, status))
                throw new InvalidOperationException("Invalid status transition");

            order.Status = (int)status;
            UpdateStatusTimestamps(order, status);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<OrderDto>(order);
        }

        private bool IsValidStatusTransition(OrderStatus current, OrderStatus newStatus)
        {
            return newStatus switch
            {
                OrderStatus.Approved => current == OrderStatus.Pending,
                OrderStatus.Rejected => current == OrderStatus.Pending,
                OrderStatus.Prepared => current == OrderStatus.Approved,
                OrderStatus.Shipped => current == OrderStatus.Prepared,
                OrderStatus.Delivered => current == OrderStatus.Shipped,
                OrderStatus.Completed => current == OrderStatus.Delivered,
                _ => false
            };
        }

        private void UpdateStatusTimestamps(Order order, OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Approved:
                    order.ApprovalDate = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliverDate = DateTime.UtcNow;
                    break;
            }
        }

        public async Task<Order> CompleteOrder(Guid orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");

            if (order.Status == (int)OrderStatus.Completed) return order;

            order.Status = (int)OrderStatus.Completed;
            order.DeliverDate = DateTime.UtcNow;

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CommitAsync();

            // Generate invoice
            await _invoiceService.GenerateInvoiceForOrderAsync(orderId);
            return order;
        }

        // Implement other methods...
    }
}
