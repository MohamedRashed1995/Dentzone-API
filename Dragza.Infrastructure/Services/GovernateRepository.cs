using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class GovernateRepository : Repository<Governate>, IGovernateRepository
    {
        public GovernateRepository(DragzaContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var x = await _context.Governates.Where(g => g.Id == id).FirstOrDefaultAsync();
            if (x == null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<Governate>> GetAllWithRegionId(Guid regionId)
        {
            return await _context.Governates
                .Where(g => g.RegionId == regionId && (g.IsDeleted == null || g.IsDeleted == false))
                .ToListAsync();
        }

        public async Task<bool> ChangeStatus(Guid id)
        {
            var governate = await _context.Governates.FindAsync(id);
            if (governate == null)
            {
                return false;
            }

            if (governate.IsDeleted == null)
                governate.IsDeleted = true; // Toggle the status
            else
                governate.IsDeleted = !governate.IsDeleted;
            _context.Update(governate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
