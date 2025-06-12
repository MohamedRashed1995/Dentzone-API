using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(DragzaContext context) : base(context) { }

        public async Task<Invoice> GetByIdAsync(Guid id)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceType)
                .Include(i => i.Order)
                    .ThenInclude(o => o.PharmacyUser)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceType)
                .Include(i => i.Order)
                    .ThenInclude(o => o.PharmacyUser)
                .FirstOrDefaultAsync(i => i.OrderId == orderId);
        }

        public async Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceType)
                .Include(i => i.Order)
                .Where(i => i.PharmacyUserId == userId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Invoice>> GetByInvintoryIdAsync(Guid userId)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceType)
                .Include(i => i.Order)
                .Where(i => i.Order.InventoryUserId == userId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceType)
                .Include(i => i.Order)
                    .ThenInclude(o => o.PharmacyUser)
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<Invoice> AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var invoice = await GetByIdAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsForOrderAsync(Guid orderId)
        {
            return await _context.Invoices.AnyAsync(i => i.OrderId == orderId);
        }
    }
}
