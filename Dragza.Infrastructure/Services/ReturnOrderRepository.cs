

using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO.ReturnOrder;
using Dragza.Domain.Models;
using Dragza.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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
                .Include(ro => ro.UserId)
                .Include(ro => ro.InventoryUser)
                .FirstOrDefaultAsync(ro => ro.Id == id);
        }

        public async Task<IEnumerable<ReturnOrder>> GetByPharmacyAsync(Guid UserId)
        {
            return await _context.ReturnOrders
                .Where(ro => ro.UserId == UserId && ro.InventoryUserId == null)
                .Include(ro => ro.ReturnedItems)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReturnOrder>> GetByInventoryUserAsync(Guid inventoryUserId)
        {
            return await _context.ReturnOrders
                .Include(a=>a.InventoryUser)
              .Include(a=>a.UserId)
                .Include(ro => ro.ReturnedItems)
                  .Where(ro => ro.InventoryUserId == inventoryUserId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReturnOrder>> GetAllWithDetailsAsync()
        {
			

			return await _context.ReturnOrders
                .Include(ro => ro.Order)
				.Include(ro => ro.User)
				.Include(ro => ro.InventoryUser)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Product)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Reason)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.ProductPrice)
                //.Where(x => x.InventoryUserId == null || x.InventoryUserId == Guid.Empty)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ReturnOrder?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ReturnOrders
                .Include(ro => ro.Order)
                .Include(ro => ro.User)
                .Include(ro => ro.InventoryUser)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Product)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.Reason)
                .Include(ro => ro.ReturnedItems)
                    .ThenInclude(ri => ri.ProductPrice)
                .FirstOrDefaultAsync(ro => ro.Id == id);
        }


        public async Task<ReturnOrder?> GetByIdIncludeAsync(
    Guid id,
    Func<IQueryable<ReturnOrder>, IIncludableQueryable<ReturnOrder, object>> include = null)
        {
            IQueryable<ReturnOrder> query = _context.ReturnOrders.AsQueryable();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }


    }
}
