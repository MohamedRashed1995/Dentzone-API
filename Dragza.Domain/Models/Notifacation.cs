using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.Models
{
    public class Notifacation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
       
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
