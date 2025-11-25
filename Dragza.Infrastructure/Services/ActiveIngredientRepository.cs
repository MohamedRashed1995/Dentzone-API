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
    public class ActiveIngredientRepository : Repository<ActiveIngredient>, IActiveIngredientRepository
    {
        private readonly DragzaContext _context;

        public ActiveIngredientRepository(DragzaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ActiveIngredient> GetByName(string name)
        {
            var active = await _context.ActiveIngredients.Where(a => a.Name.Contains(name)).FirstOrDefaultAsync();
            return active;
        }
    }
}
