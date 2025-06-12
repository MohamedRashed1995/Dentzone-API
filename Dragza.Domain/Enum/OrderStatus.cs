using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.Enum
{
    public enum OrderStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Prepared = 3,
        Shipped = 4,
        Delivered = 5,
        Completed = 6
    }
}
