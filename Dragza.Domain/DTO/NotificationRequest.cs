using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class NotificationRequest
    {
        public string DeviceKey { get; set; }
        public Guid ClientId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public object? Data { get; set; }
        public int Type { get; set; }
    }
}
