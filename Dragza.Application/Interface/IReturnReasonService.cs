using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IReturnReasonService
    {
        Task<ReturnReasonDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ReturnReasonDto>> GetAllAsync();
        Task<ReturnReasonDto> CreateAsync(CreateReturnReasonDto createDto);
        Task UpdateAsync(Guid id, UpdateReturnReasonDto updateDto);
        Task DeleteAsync(Guid id);
    }
}
