using Dragza.Domain.DTO.ReturnOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IReturnOrderService
    {
        Task<ReturnOrderDto> CreateReturnAsync(CreateReturnOrderDto dto, Guid pharmacyUserId);
        Task<ReturnOrderDto> UpdateReturnStatusAsync(Guid returnId, UpdateReturnStatusDto dto, Guid userId);
        Task<ReturnOrderDto> GetReturnAsync(Guid returnId);
        Task<IEnumerable<ReturnOrderDto>> GetPharmacyReturnsAsync(Guid pharmacyId);
        Task<IEnumerable<ReturnOrderDto>> GetVendorReturnsAsync(Guid vendorId);
        Task<IEnumerable<ReturnReasonDto>> GetReturnReasonsAsync();
    }
}
