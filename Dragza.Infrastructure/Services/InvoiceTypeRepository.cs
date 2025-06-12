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
    public class InvoiceTypeRepository : Repository<InvoiceType>, IInvoiceTypeRepository
    {
        public InvoiceTypeRepository(DragzaContext context) : base(context)
        {
        }

        public async Task<InvoiceType> GetByNameAsync(string Name)
        {
            return await _context.InvoiceTypes.Where(x => x.Name == Name).FirstOrDefaultAsync();
        }
    }
}
