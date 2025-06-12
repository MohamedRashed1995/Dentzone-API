using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IRegionRepository : IRepository<Region>
    {
        Task<Guid?> FindRegionIdAsync(string regionName, string districtName, Guid? governateId, Guid? cityId);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<User>> GetUsersByRegionAsync(Guid regionId);

    }
}
