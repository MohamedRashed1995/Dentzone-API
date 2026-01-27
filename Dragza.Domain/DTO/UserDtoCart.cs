using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class UserDtoCart
    {
        public Guid Id { get; set; }

        public string? BussinesName { get; set; }

        public bool? IsPharmacy { get; set; }

        public string? Photo { get; set; }

        public bool? IsActive { get; set; }

        public string? UserName { get; set; }
        public decimal? MinOrder { get; set; }

    }
}
