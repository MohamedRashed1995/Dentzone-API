using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO.Order;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using Dragza.Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

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

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IBalanceService balanceService, IInvoiceService invoiceService, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _balanceService = balanceService;
            _invoiceService = invoiceService;
            _logger = logger;
        }
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto, Guid pharmacyUserId)
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
                long uniqueNum = UniqueNumberGenerator.GenerateUniqueNumber();

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
                    OrderNumber = uniqueNum.ToString(),
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
              await CreateOrdersByInventoryAsync(orderDto, pharmacyUserId , uniqueNum);
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
                                x => x.InventoryUserId == null,
                                include: q => q.Include(o => o.OrderItems)
                              .ThenInclude(oi => oi.Product)
                              .ThenInclude(oi => oi.ProductPrices)
                              .ThenInclude(oi => oi.InventoryUser)
                              .Include(o => o.InventoryUser)
                              .Include(o => o.PharmacyUser)
            //.ThenInclude(oi => oi.ProductPrice)
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

		//public async Task ReAssignOrder(ReAssignOrder reAssignOrderDto)
		//{
		//    var order = await _unitOfWork.OrderRepository.GetByIdAsync(reAssignOrderDto.OrderId);
		//    order.InventoryUserId = reAssignOrderDto.UserId;
		//    _unitOfWork.OrderRepository.Update(order);
		//    await _unitOfWork.SaveChangesAsync();
		//}
		public async Task<object> ReAssignOrder(ReAssignOrder reAssignOrderDto)
		{
            try
            {
                _logger.LogInformation("ReAssignOrder");

                // Get the original order with its items
                var originalOrder = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(reAssignOrderDto.OrderId);
                if (originalOrder == null)
                {
                    throw new InvalidOperationException("Order not found");
                }

                // Check if the new userId is the same as current InventoryUserId
                if (originalOrder.InventoryUserId == reAssignOrderDto.UserId)
                {
                    return new { Message = "This order already exists" };
                }

                // Get the order items to be transferred
                var itemsToTransfer = originalOrder.OrderItems.ToList();
                if (reAssignOrderDto.OrderItemIds != null && reAssignOrderDto.OrderItemIds.Any())
                {
                    // Transfer only specific items
                    itemsToTransfer = originalOrder.OrderItems
                        .Where(oi => reAssignOrderDto.OrderItemIds.Contains(oi.Id))
                        .ToList();
                }

                // Calculate total amount for items being transferred
                decimal transferAmount = itemsToTransfer.Sum(item => item.Amount * item.Quantity);

                // Create new order
                var newOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    PharmacyUserId = originalOrder.PharmacyUserId,
                    OrderDate = DateTime.UtcNow,
                    Status = (int)OrderStatus.Pending,
                    TotalAmount = transferAmount,
                    InventoryUserId = reAssignOrderDto.UserId, // New inventory user
                    IsApproved = false,
                    ApprovalDate = null,
                    CreditUsed = originalOrder.CreditUsed,
                    CashPaid = originalOrder.CashPaid,
                    CreditAccountId = originalOrder.CreditAccountId,
                    CouponId = originalOrder.CouponId
                };

                // Add new order to database
                await _unitOfWork.OrderRepository.AddAsync(newOrder);
                await _unitOfWork.SaveChangesAsync(); // Save to get the new OrderId

                // Update OrderId for transferred items
                foreach (var item in itemsToTransfer)
                {
                    item.OrderId = newOrder.Id; // Change OrderId to new order
                    _unitOfWork.OrderItemRepository.Update(item);
					await _unitOfWork.SaveChangesAsync();
				}
				var remainingItems = await _unitOfWork.OrderItemRepository.GetItemsByOrderIdAsync(originalOrder.Id);

				if (remainingItems.Count()==0) // No items found
				{
					originalOrder.Status = (int)OrderStatus.ReAssignTo;
					originalOrder.TotalAmount = 0;
				}
				else
				{
					// Items still remain - keep original status, recalculate total
					originalOrder.TotalAmount = remainingItems.Sum(item => item.Amount * item.Quantity);
					// originalOrder.Status stays unchanged
				}

				_unitOfWork.OrderRepository.Update(originalOrder);
                await _unitOfWork.SaveChangesAsync();
                
                return new
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("ReAssignOrder :: ", ex);
                throw;
            }
		}
        public async Task RemoveItem(RemoveItemDto removeItemDto)
        {
            var item = await _unitOfWork.OrderItemRepository.GetByIdAsync(removeItemDto.ItemId);
            var pharmacyTransaction = await _unitOfWork.BalanceTransactionRepository.GetAllAsync(
                t => t.OrderId == removeItemDto.OrderId
            );
            if (pharmacyTransaction.Any())
            {
                var transaction = pharmacyTransaction.FirstOrDefault();
                if (transaction != null)
                {
                    transaction.Amount = transaction.Amount + ((item.ProductPrice.PurchasePrice??0) * item.Quantity);
                    _unitOfWork.BalanceTransactionRepository.Update(transaction);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            if (item.OrderId == removeItemDto.OrderId)
            {
                _unitOfWork.OrderItemRepository.Delete(item);
                _unitOfWork.SaveChangesAsync();
            }
            var relatedOrder = await _unitOfWork.OrderRepository.GetAllAsync(o => o.OrderNumber == removeItemDto.OrderNumber);
            if (relatedOrder.FirstOrDefault() != null)
            {
                foreach (var order in relatedOrder)
                {
                    var orderItem = await _unitOfWork.OrderItemRepository.
                        GetAllAsync(o => o.ProductPriceId == item.ProductPriceId && o.OrderId == order.Id);
                    if (orderItem.FirstOrDefault() != null)
                    {
                        _unitOfWork.OrderItemRepository.Delete(orderItem.FirstOrDefault());
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                relatedOrder.FirstOrDefault().TotalAmount = relatedOrder.FirstOrDefault().OrderItems.Sum(i => i.Amount * i.Quantity);
                _unitOfWork.OrderRepository.Update(relatedOrder.FirstOrDefault());
            }

            var orderToUpdate = await _unitOfWork.OrderRepository.GetByIdAsync(removeItemDto.OrderId);
            orderToUpdate.TotalAmount = orderToUpdate.OrderItems.Sum(i => i.Amount * i.Quantity);
            _unitOfWork.OrderRepository.Update(orderToUpdate);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();


        }

        public async Task<OrderDto> UpdateOrderStatusAsync(Guid itemId, OrderStatus status, Guid userId)
        {
            var item = await _unitOfWork.OrderItemRepository.GetAllAsync(i => i.Id == itemId ,
                include: x => x.Include(o=> o.Order));
            if (item == null) throw new KeyNotFoundException("item not found");

            // Authorization check
            //if (order.InventoryUserId != userId)
            //    throw new UnauthorizedAccessException("Not authorized to modify this order");

            if (!IsValidStatusTransition((OrderStatus)item.FirstOrDefault().Status, status))
                throw new InvalidOperationException("Invalid status transition");

            item.FirstOrDefault().Status = (int)status;
            //UpdateStatusTimestamps(order, status);
             _unitOfWork.OrderItemRepository.Update(item.FirstOrDefault());
            await _unitOfWork.SaveChangesAsync();

            var order = await _unitOfWork.OrderRepository
                .GetAllAsync(x =>
                x.OrderNumber == item.FirstOrDefault().Order.OrderNumber
                && x.InventoryUserId == null,
                include: q => q.Include(o => o.OrderItems)
                );
            if (order == null || !order.Any())
                throw new KeyNotFoundException("item not found");
            var pharmacyItem = order.FirstOrDefault().OrderItems.Where(x => x.ProductPriceId == item.FirstOrDefault().ProductPriceId).FirstOrDefault();
            pharmacyItem.Status = (int)status;
            _unitOfWork.OrderItemRepository.Update(pharmacyItem);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<OrderDto>(item.FirstOrDefault().Order);
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

        private async Task<List<OrderDto>> CreateOrdersByInventoryAsync(CreateOrderDto orderDto, Guid pharmacyUserId , long uniqueNum)
        {
            try
            {
                // Validate user
                var user = await _unitOfWork.UserRepository.GetByIdAsync(pharmacyUserId);
                if (user == null || user.IsPharmacy != true)
                    throw new InvalidOperationException("Only pharmacy users can create orders");

                // Get product prices to determine inventory
                var productPriceIds = orderDto.Items.Select(i => i.ProductPriceId).Distinct().ToList();
                var productPrices = await _unitOfWork.ProductPriceRepository.GetAllAsync(o => productPriceIds.Any(x => x == o.Id));

                // Create dictionary for lookup (handle nullable InventoryUserId)
                var priceToInventoryDict = productPrices.ToDictionary(
                    pp => pp.Id,
                    pp => (Guid?)pp.InventoryUserId  // Cast to Guid? to handle nulls
                );

                // Validate all product prices exist
                var missingIds = orderDto.Items
                    .Select(i => i.ProductPriceId)
                    .Except(productPrices.Select(pp => pp.Id))
                    .ToList();

                if (missingIds.Any())
                    throw new KeyNotFoundException($"Missing product prices for IDs: {string.Join(", ", missingIds)}");

                // Group items by inventory
                var groupedItems = orderDto.Items
                    .GroupBy(item => priceToInventoryDict.GetValueOrDefault(item.ProductPriceId))
                    .ToList();

                // Validate coupon usage
                if (orderDto.CouponId.HasValue && groupedItems.Count > 1)
                    throw new InvalidOperationException("Coupons cannot be applied to orders spanning multiple inventories");

                decimal originalTotal = orderDto.Items.Sum(i => i.Quantity * i.UnitPrice);
                decimal entireTotalAfterCoupon = originalTotal;
                Coupon coupon = null;

                // Apply coupon if exists
                if (orderDto.CouponId.HasValue)
                {
                    coupon = await _unitOfWork.CouponRepository.GetByIdAsync(orderDto.CouponId.Value);
                    if (coupon != null && coupon.IsActive != false)
                        entireTotalAfterCoupon -= coupon.DiscountValue;
                }

                // Calculate payment distribution
                (decimal entireCredit, decimal entireCash) = CalculatePaymentDistribution(
                    orderDto.PaymentMethod,
                    entireTotalAfterCoupon,
                    orderDto.CreditAmount
                );

                // Validate credit balance
                if (entireCredit > 0)
                {
                    var creditAccount = (await _balanceService.GetUserBalances(pharmacyUserId)).CreditAccount;
                    if (!await _balanceService.HasSufficientBalance(creditAccount.Id, -entireCredit))
                        throw new InvalidOperationException("Insufficient credit balance");
                }

                var orders = new List<OrderDto>();
                decimal totalAssignedGroupTotalAfterCoupon = 0;
                decimal totalAssignedCredit = 0;
                int groupCount = groupedItems.Count;

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        for (int i = 0; i < groupCount; i++)
                        {
                            var group = groupedItems[i];
                            var inventoryId = group.Key;
                            var items = group.ToList();
                            decimal groupTotalOriginal = items.Sum(i => i.Quantity * i.UnitPrice);
                            decimal ratio = groupTotalOriginal / originalTotal;

                            // Calculate group's total after coupon (proportional discount)
                            decimal groupTotalAfterCoupon;
                            if (i == groupCount - 1)
                            {
                                groupTotalAfterCoupon = entireTotalAfterCoupon - totalAssignedGroupTotalAfterCoupon;
                            }
                            else
                            {
                                groupTotalAfterCoupon = Math.Round(entireTotalAfterCoupon * ratio, 2);
                            }
                            totalAssignedGroupTotalAfterCoupon += groupTotalAfterCoupon;

                            // Calculate group's credit portion
                            decimal groupCredit;
                            if (i == groupCount - 1)
                            {
                                groupCredit = entireCredit - totalAssignedCredit;
                            }
                            else
                            {
                                groupCredit = Math.Round(entireCredit * ratio, 2);
                            }
                            totalAssignedCredit += groupCredit;

                            // Calculate group's cash portion
                            decimal groupCash = groupTotalAfterCoupon - groupCredit;

                            // Create order for this inventory group
                            var order = await CreateInventoryOrder(
                                orderDto,
                                pharmacyUserId,
                                inventoryId,
                                items,
                                groupTotalAfterCoupon,
                                groupCredit,
                                groupCash,
                                uniqueNum,
                                coupon
                            );

                            orders.Add(order);
                        }

                        await _unitOfWork.CommitAsync();
                        await transaction.CommitAsync();
                        return orders;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateOrdersByInventoryAsync error: {Message}", ex.Message);
                throw;
            }
        }

        private (decimal Credit, decimal Cash) CalculatePaymentDistribution(
            PaymentMethod method,
            decimal totalAfterCoupon,
            decimal? creditAmount)
        {
            return method switch
            {
                PaymentMethod.Cash => (0, totalAfterCoupon),
                PaymentMethod.Credit => (totalAfterCoupon, 0),
                PaymentMethod.Mixed => (
                    creditAmount ?? throw new InvalidOperationException("Credit amount required"),
                    totalAfterCoupon - creditAmount.Value
                ),
                _ => throw new InvalidOperationException("Invalid payment method")
            };
        }

        private async Task<OrderDto> CreateInventoryOrder(
            CreateOrderDto orderDto,
            Guid pharmacyUserId,
            Guid? inventoryId,
            List<OrderItemDto> items,
            decimal groupTotalAfterCoupon,
            decimal groupCredit,
            decimal groupCash,
            long uniqueNum,
            Coupon coupon)
        {
            // Determine credit account if needed
            Guid? creditAccountId = null;
            if (groupCredit > 0)
            {
                var creditAccount = (await _balanceService.GetUserBalances(pharmacyUserId)).CreditAccount;
                creditAccountId = creditAccount?.Id;
            }

            // Generate unique order number
            //long uniqueNum = UniqueNumberGenerator.GenerateUniqueNumber();

            // Create order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                PharmacyUserId = pharmacyUserId,
                InventoryUserId = inventoryId,
                OrderDate = DateTime.UtcNow,
                Status = (int)OrderStatus.Pending,
                TotalAmount = groupTotalAfterCoupon,
                CreditUsed = groupCredit,
                CashPaid = groupCash,
                CreditAccountId = creditAccountId,
                OrderNumber = uniqueNum.ToString(),
                CouponId = coupon?.Id
            };

            // Add items
            foreach (var item in items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Amount = item.Quantity,
                    ProductPriceId = item.ProductPriceId
                });
            }

            await _unitOfWork.OrderRepository.AddAsync(order);

            // Process credit transaction if needed
            //if (groupCredit > 0 && creditAccountId.HasValue)
            //{
            //    await _balanceService.CreateTransaction(
            //        creditAccountId.Value,
            //        -groupCredit,
            //        pharmacyUserId,
            //        TransactionType.Payment,
            //        $"Order payment #{order.Id}",
            //        order.Id
            //    );
            //}

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<List<OrderDto>> GetRelatedOrdersAsync(string orderNumber)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                            o => o.OrderNumber == orderNumber,
                            include: q => q.Include(o => o.OrderItems)
                                          .ThenInclude(oi => oi.Product)
                                          .Include(u => u.InventoryUser)
                                          .Include(u => u.PharmacyUser)
                                          .Include(o => o.OrderItems)
                                          .ThenInclude(oi => oi.ProductPrice)
                        );
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<(bool Success, string Message)> CreateOrderByIdAsync(string userId)
        {
            var orderStatus = await _unitOfWork.OrderRepository.CreateOrderById(userId);
            if (orderStatus.Success)
            {
                return (orderStatus.Success,orderStatus.Message);
            }
            return (false, orderStatus.Message);
        }
    }
}
