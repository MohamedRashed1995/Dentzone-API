using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class ReturnedItemRepository : Repository<ReturnedItem>, IReturnedItemRepository
    {
        public ReturnedItemRepository(DragzaContext context) : base(context)
        {
        }
    }
}
