using Dragza.Domain.DTO.Order;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, Guid pharmacyUserId);
        Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus status, Guid userId);
        Task<OrderDto> GetOrderByIdAsync(Guid orderId);
        Task<List<OrderDto>> GetUserOrdersAsync(Guid userId);
        Task<List<OrderDto>> GetVendorOrdersAsync(Guid vendorId);

        Task ReAssignOrder(ReAssignOrder reAssignOrderDto);
        Task RemoveItem(RemoveItemDto removeItemDto);
        Task<Order> CompleteOrder(Guid orderId);
        Task<List<OrderDto>> GetAllorders();
    }
}
