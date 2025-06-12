using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO.Order
{
    public class RemoveItemDto
    {
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
    }
}
