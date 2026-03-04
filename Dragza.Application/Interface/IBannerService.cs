using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IBannerService
    {
        Task<IEnumerable<BannerDto>> GetAllAsync();
        Task<BannerDto?> GetByIdAsync(Guid id);
        Task<BannerDto> CreateAsync(BannerDto dto);
        Task<bool> UpdateAsync(Guid id, BannerDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
