using Dragza.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO.Order
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
