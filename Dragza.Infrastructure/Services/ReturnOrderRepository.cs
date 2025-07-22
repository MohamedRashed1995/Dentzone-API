

using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Dragza.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Dragaza.Infrastructure.Repositories
{
    public class ReturnOrderRepository : Repository<ReturnOrder>, IReturnOrderRepository
    {
        private readonly DragzaContext _context;

        public ReturnOrderRepository(DragzaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ReturnOrder> GetWithItemsAsync(Guid id)
        {
            return await _context.ReturnOrders
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Product)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.ProductPrice)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Reason)
                .Include(ro => ro.PharmacyUser)
                .Include(ro => ro.InventoryUser)
                .FirstOrDefaultAsync(ro => ro.Id == id);
        }

        public async Task<IEnumerable<ReturnOrder>> GetByPharmacyAsync(Guid pharmacyId)
        {
            return await _context.ReturnOrders
                .Where(ro => ro.PharmacyUserId == pharmacyId)
                .Include(ro => ro.ReturnedItems)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReturnOrder>> GetByInventoryUserAsync(Guid inventoryUserId)
        {
            return await _context.ReturnOrders
                .Where(ro => ro.InventoryUserId == inventoryUserId)
                .Include(ro => ro.ReturnedItems)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReturnOrder>> GetAllWithDetailsAsync()
        {
            return await _context.ReturnOrders
                .Include(ro => ro.Order)
                .Include(ro => ro.PharmacyUser)
                .Include(ro => ro.InventoryUser)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Product)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Reason)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.ProductPrice)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ReturnOrder?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ReturnOrders
                .Include(ro => ro.Order)
                .Include(ro => ro.PharmacyUser)
                .Include(ro => ro.InventoryUser)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Product)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Reason)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.ProductPrice)
                .FirstOrDefaultAsync(ro => ro.Id == id);
        }
    }
}
