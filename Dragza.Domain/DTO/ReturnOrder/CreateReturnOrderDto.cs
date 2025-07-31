using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO.ReturnOrder
{
    public class CreateReturnOrderDto
    {
        public Guid OrderId { get; set; }

        public List<ReturnedItemDto> Items { get; set; } = new();
    }
}
