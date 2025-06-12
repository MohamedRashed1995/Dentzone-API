using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Dragza.Infrastructure.Services
{
    public class RegionRepository : Repository<Region>, IRegionRepository
    {

        public RegionRepository(DragzaContext context) : base(context) { }

        public async Task<Guid?> FindRegionIdAsync(
         string regionName,
         string districtName,
         Guid? governateId,
         Guid? cityId)
        {
            // 1. Try by Region Name
            if (!string.IsNullOrWhiteSpace(regionName))
            {
                var region = await _context.Regions
                    .FirstOrDefaultAsync(r => r.RegionName == regionName);

                if (region != null)
                    return region.Id;
            }

            // 2. Try by Governate ID
            if (governateId.HasValue)
            {
                var governate = await _context.Governates
                    .Include(g => g.Region)
                    .FirstOrDefaultAsync(g => g.Id == governateId.Value);

                if (governate?.Region != null)
                    return governate.Region.Id;
            }

            // 3. Try by City ID
            if (cityId.HasValue)
            {
                var city = await _context.Cities
                    .Include(c => c.Governate)
                    .ThenInclude(g => g.Region)
                    .FirstOrDefaultAsync(c => c.Id == cityId.Value);

                if (city?.Governate?.Region != null)
                    return city.Governate.Region.Id;
            }

            // 4. Try by District Name (Destrict)
            if (!string.IsNullOrWhiteSpace(districtName))
            {
                var district = await _context.Destricts
                    .Include(d => d.City)
                    .ThenInclude(c => c.Governate)
                    .ThenInclude(g => g.Region)
                    .FirstOrDefaultAsync(d => d.Name == districtName);

                if (district?.City?.Governate?.Region != null)
                    return district.City.Governate.Region.Id;
            }

            // 5. Default return if no matches found
            return null;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Regions.AnyAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<User>> GetUsersByRegionAsync(Guid regionId)
        {
            return await _context.Users
                .Where(u => u.RegionId == regionId)
                .ToListAsync();
        }
    }
}
