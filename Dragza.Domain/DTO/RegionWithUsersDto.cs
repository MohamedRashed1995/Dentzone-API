using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class RegionWithUsersDto : RegionDto
    {
        public IEnumerable<UserDto> Users { get; set; }
    }
}
