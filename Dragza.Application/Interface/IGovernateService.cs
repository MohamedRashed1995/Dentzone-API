using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IGovernateService
    {
        Task<GovernateWithCitiesDto> GetByIdAsync(Guid id);
        Task<IEnumerable<GovernateDto>> GetAllAsync(bool isActive);
        Task<GovernateDto> CreateAsync(CreateGovernateDto createDto);
        Task UpdateAsync(Guid id, UpdateGovernateDto updateDto);
        Task DeleteAsync(Guid id);
        Task<List<GovernateDto>> GetAllWithRegionId(Guid regionId);
        Task<bool> ChangeStatus(Guid id);
    }
}
