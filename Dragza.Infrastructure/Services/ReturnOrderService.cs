using AutoMapper;
using Dragaza.Infrastructure.Repositories;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.DTO.ReturnOrder;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using Dragza.Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class ReturnOrderService : IReturnOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReturnOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReturnOrderDto> CreateReturnAsync(CreateReturnOrderDto dto, Guid pharmacyUserId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(dto.OrderId);
                if (order == null) throw new KeyNotFoundException("Order not found");
                long uniqueNumber = UniqueNumberGenerator.GenerateUniqueNumber();
                //// Validate user exists
                //var userExists = await _unitOfWork.UserRepository.ExistsAsync(order.PharmacyUserId);
                //if (!userExists) throw new KeyNotFoundException("User not found");

                var returnOrder = new ReturnOrder
                {
                    Id = Guid.NewGuid(),
                    OrderId = dto.OrderId,
                    PharmacyUserId = order.PharmacyUserId,
                    RequestDate = DateTime.UtcNow,
                    Statuse = (int)ReturnOrderStatus.Requested,
                    //InventoryUserId = order.OrderItems.First().ProductPrice.InventoryUserId, // CRITICAL FIX: Add missing InventoryUserId
                    AdminApproval = false,
                    ReturnedItems = new List<ReturnedItem>(),
                    ReturnOrderNumber = uniqueNumber.ToString()
                };

                decimal totalValue = 0;

                foreach (var item in dto.Items)
                {
                    var orderItem = order.OrderItems.FirstOrDefault(oi =>
                        oi.ProductId == item.ProductId &&
                        oi.ProductPriceId == item.ProductPriceId);

                    if (orderItem == null || item.QuantityReturned > orderItem.Quantity)
                        throw new InvalidOperationException("Invalid return quantity");

                    var reason = await _unitOfWork.ReturnReasonRepository.GetByIdAsync(item.ReasonId);
                    if (reason == null) throw new KeyNotFoundException("Invalid return reason");

                    var returnedItem = new ReturnedItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        ProductPriceId = item.ProductPriceId,
                        QuantityReturned = item.QuantityReturned,
                        ReasonId = item.ReasonId,
                        OtherReason = item.OtherReason,
                        TotalAmount = item.QuantityReturned * orderItem.Amount / orderItem.Quantity,
                        ReturnOrderId = returnOrder.Id,
                        OrderId = order.Id // CRITICAL FIX: Add missing OrderId
                    };

                    totalValue += returnedItem.TotalAmount;
                    returnOrder.ReturnedItems.Add(returnedItem);
                }

                returnOrder.TotalReturnValue = totalValue;

                await _unitOfWork.ReturnOrderRepository.AddAsync(returnOrder);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                await CreateReturnOrdersByInventoryAsync(dto, pharmacyUserId,uniqueNumber);
                return _mapper.Map<ReturnOrderDto>(returnOrder);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<ReturnOrderDto>> GetPharmacyReturnsAsync(Guid pharmacyId)
        {
            var returns = await _unitOfWork.ReturnOrderRepository.GetByPharmacyAsync(pharmacyId);
            return _mapper.Map<IEnumerable<ReturnOrderDto>>(returns);
        }

        public async Task<ReturnOrderDto> GetReturnAsync(Guid returnId)
        {
            var returnOrder = await _unitOfWork.ReturnOrderRepository.GetWithItemsAsync(returnId);
            return _mapper.Map<ReturnOrderDto>(returnOrder);
        }

        public async Task<IEnumerable<ReturnReasonDto>> GetReturnReasonsAsync()
        {
            var reasons = await _unitOfWork.ReturnReasonRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReturnReasonDto>>(reasons);
        }

        public async Task<IEnumerable<ReturnOrderDto>> GetVendorReturnsAsync(Guid vendorId)
        {
            var returns = await _unitOfWork.ReturnOrderRepository.GetByInventoryUserAsync(vendorId);
            return _mapper.Map<IEnumerable<ReturnOrderDto>>(returns);
        }

        public async Task<ReturnOrderDto> UpdateReturnStatusAsync(Guid returnId, UpdateReturnStatusDto dto, Guid userId)
        {
            var returnOrder = await _unitOfWork.ReturnOrderRepository.GetByIdAsync(returnId);
            if (returnOrder == null) throw new KeyNotFoundException("Return order not found");

            // Authorization check
            if (returnOrder.InventoryUserId != userId)
                throw new UnauthorizedAccessException("Not authorized to update this return");

            if (!IsValidStatusTransition((ReturnOrderStatus)returnOrder.Statuse, dto.Status))
                throw new InvalidOperationException("Invalid status transition");

            returnOrder.Statuse = (int)dto.Status;
            UpdateStatusMetadata(returnOrder, dto.Status);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ReturnOrderDto>(returnOrder);
        }

        public async Task<IEnumerable<ReturnOrderDto>> GetAllReturnOrdersAsync()
        {
            var returnOrders = await _unitOfWork.ReturnOrderRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReturnOrderDto>>(returnOrders);
        }

        public async Task<ReturnOrderDto?> GetReturnOrderByIdAsync(Guid id)
        {
            var returnOrder = await _unitOfWork.ReturnOrderRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ReturnOrderDto>(returnOrder);
        }

        Task<IEnumerable<ReturnReasonDto>> IReturnOrderService.GetReturnReasonsAsync()
        {
            throw new NotImplementedException();
        }

        private bool IsValidStatusTransition(ReturnOrderStatus current, ReturnOrderStatus newStatus)
        {
            return newStatus switch
            {
                ReturnOrderStatus.Approved => current == ReturnOrderStatus.Requested,
                ReturnOrderStatus.Rejected => current == ReturnOrderStatus.Requested,
                ReturnOrderStatus.Processing => current == ReturnOrderStatus.Approved,
                ReturnOrderStatus.Completed => current == ReturnOrderStatus.Processing,
                _ => false
            };
        }

        private void UpdateStatusMetadata(ReturnOrder returnOrder, ReturnOrderStatus status)
        {
            switch (status)
            {
                case ReturnOrderStatus.Approved:
                    returnOrder.ApprovalDate = DateTime.UtcNow;
                    returnOrder.AdminApproval = true;
                    break;
                case ReturnOrderStatus.Completed:
                    returnOrder.Order.DeliverDate = DateTime.UtcNow;
                    break;
            }
        }

        private async Task<List<ReturnOrderDto>> CreateReturnOrdersByInventoryAsync(CreateReturnOrderDto dto, Guid pharmacyUserId,long uniqueNumber)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            var returnOrders = new List<ReturnOrderDto>();

            try
            {
                // Fetch original order with items and product prices
                var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(dto.OrderId);
                if (order == null) throw new KeyNotFoundException("Order not found");

                // Validate user authorization
                if (order.PharmacyUserId != pharmacyUserId)
                    throw new UnauthorizedAccessException("User not authorized to return this order");

                // Create dictionary to map product prices to inventory IDs
                var inventoryMap = order.OrderItems
                    .Select(oi => oi.ProductPrice)
                    .Distinct()
                    .ToDictionary(pp => pp.Id, pp => pp.InventoryUserId);

                // Group return items by inventory
                var groupedItems = dto.Items
                    .GroupBy(item =>
                    {
                        if (inventoryMap.TryGetValue(item.ProductPriceId, out var inventoryId))
                            return inventoryId;
                        throw new KeyNotFoundException($"Product price {item.ProductPriceId} not found in order");
                    })
                    .ToList();

                // Process each inventory group separately
                foreach (var group in groupedItems)
                {
                    var inventoryId = group.Key;
                    var returnOrder = await ProcessReturnGroup(
                        order,
                        pharmacyUserId,
                        inventoryId,
                        group.ToList(),
                        inventoryMap,
                        uniqueNumber
                    );

                    returnOrders.Add(returnOrder);
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return returnOrders;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<ReturnOrderDto> ProcessReturnGroup(
            Order order,
            Guid pharmacyUserId,
            Guid inventoryId,
            List<ReturnedItemDto> items,
            Dictionary<Guid, Guid> inventoryMap
            ,long uniqueNumber)
        {
            // Create return order
            var returnOrder = new ReturnOrder
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                PharmacyUserId = pharmacyUserId,
                RequestDate = DateTime.UtcNow,
                Statuse = (int)ReturnOrderStatus.Requested,
                InventoryUserId = inventoryId,
                AdminApproval = false,
                ReturnedItems = new List<ReturnedItem>(),
                ReturnOrderNumber = uniqueNumber.ToString()
            };

            decimal totalValue = 0;

            foreach (var item in items)
            {
                // Find matching order item
                var orderItem = order.OrderItems.FirstOrDefault(oi =>
                    oi.ProductId == item.ProductId &&
                    oi.ProductPriceId == item.ProductPriceId);

                if (orderItem == null)
                    throw new InvalidOperationException($"Product {item.ProductId} not found in order");

                // Validate quantity
                if (item.QuantityReturned <= 0 || item.QuantityReturned > orderItem.Quantity)
                    throw new InvalidOperationException($"Invalid return quantity for product {item.ProductId}");

                // Get return reason
                var reason = await _unitOfWork.ReturnReasonRepository.GetByIdAsync(item.ReasonId);
                if (reason == null) throw new KeyNotFoundException($"Invalid return reason: {item.ReasonId}");

                // Calculate return amount (proportional to original price)
                decimal unitPrice = orderItem.Amount / orderItem.Quantity;
                decimal returnAmount = item.QuantityReturned * unitPrice;

                // Create returned item
                var returnedItem = new ReturnedItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductPriceId = item.ProductPriceId,
                    QuantityReturned = item.QuantityReturned,
                    ReasonId = item.ReasonId,
                    OtherReason = item.OtherReason,
                    TotalAmount = returnAmount,
                    ReturnOrderId = returnOrder.Id,
                    OrderId = order.Id,
                    Status = (int)ReturnOrderStatus.Requested
                };

                totalValue += returnAmount;
                returnOrder.ReturnedItems.Add(returnedItem);
            }

            returnOrder.TotalReturnValue = totalValue;
            await _unitOfWork.ReturnOrderRepository.AddAsync(returnOrder);

            return _mapper.Map<ReturnOrderDto>(returnOrder);
        }

        public async Task<IEnumerable<ReturnOrderDto>> GetRelatedReturns(string returnNumber)
        {
            var returns = await _unitOfWork.ReturnOrderRepository
                .GetAllAsync(
                o => o.ReturnOrderNumber == returnNumber,
                include: o => o
                    .Include(ro => ro.Order)
                    .Include(ro => ro.PharmacyUser)
                    .Include(ro => ro.InventoryUser)
                    .Include(ro => ro.ReturnedItems)
                        .ThenInclude(ri => ri.Product)
                    .Include(ro => ro.ReturnedItems)
                        .ThenInclude(ri => ri.Reason)
                    .Include(ro => ro.ReturnedItems)
                        .ThenInclude(ri => ri.ProductPrice)
                );
            return _mapper.Map<IEnumerable<ReturnOrderDto>>(returns);
        }
    }
}
