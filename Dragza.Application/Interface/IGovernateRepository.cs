using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IGovernateRepository : IRepository<Governate>
    {
        Task<bool> ExistsAsync(Guid id);
        Task<List<Governate>> GetAllWithRegionId(Guid regionId);
        Task<bool> ChangeStatus(Guid id);

    }
}
