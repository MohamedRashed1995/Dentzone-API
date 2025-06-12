using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.Enum
{
    public enum ReturnOrderStatus
    {
        Requested = 0,
        Approved = 1,
        Rejected = 2,
        Processing = 3,
        Completed = 4
    }
}
