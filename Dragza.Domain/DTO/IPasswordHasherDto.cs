using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public interface IPasswordHasherDto
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
