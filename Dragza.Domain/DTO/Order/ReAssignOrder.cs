using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO.Order
{
    public class ReAssignOrder
    {
        public Guid OrderId { get; set; }
        public  Guid  UserId { get; set; }

        public List<Guid> OrderItemIds { get; set; } 
    }
}
