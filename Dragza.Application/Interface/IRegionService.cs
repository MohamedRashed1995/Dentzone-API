using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IRegionService
    {
        Task<Guid?> GetRegionIdAsync(
            string regionName,
            string districtName,
            Guid? governateId,
            Guid? cityId
        );

        Task<RegionWithUsersDto> GetByIdAsync(Guid id);
        Task<IEnumerable<RegionDto>> GetAllAsync(bool isActive);
        Task<RegionDto> CreateAsync(CreateRegionDto createDto);
        Task UpdateAsync(Guid id, UpdateRegionDto updateDto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<UserDto>> GetUsersByRegionAsync(Guid regionId);
        Task<bool> ChangeStatus(Guid id);

    }
}
