using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO.ReturnOrder;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
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

                var returnOrder = new ReturnOrder
                {
                    Id = Guid.NewGuid(),
                    OrderId = dto.OrderId,
                    PharmacyUserId = pharmacyUserId,
                    RequestDate = DateTime.UtcNow,
                    Statuse = (int)ReturnOrderStatus.Requested,
                    ReturnedItems = new List<ReturnedItem>()
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
                        ReturnOrderId = returnOrder.Id
                    };

                    totalValue += returnedItem.TotalAmount;
                    returnOrder.ReturnedItems.Add(returnedItem);
                    await _unitOfWork.ReturnedItemRepository.AddAsync(returnedItem);
                }

                returnOrder.TotalReturnValue = totalValue;
                await _unitOfWork.ReturnOrderRepository.AddAsync(returnOrder);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

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
            return _mapper.Map<ReturnOrderDto>(returnOrder);
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
    }
}
