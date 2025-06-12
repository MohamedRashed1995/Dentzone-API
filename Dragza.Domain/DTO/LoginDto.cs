using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
