using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UpdateCreditLimitDto
    {
        public Guid AccountId { get; set; }
        public decimal NewLimit { get; set; }
    }
}
