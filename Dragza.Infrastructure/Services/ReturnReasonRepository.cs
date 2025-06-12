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
    public class ReturnReasonRepository : Repository<ReturnReason>, IReturnReasonRepository
    {
        public ReturnReasonRepository(DragzaContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ReturnReason>> GetDefaultReasonsAsync()
        {
            return await _context.ReturnReasons
                .Where(r => r.Reason != "Other")
                .ToListAsync();
        }
    }
}
