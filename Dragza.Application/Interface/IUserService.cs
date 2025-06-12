using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterUserAsync(CreateUserDto createUserDto);
        Task<JWTTokenDTO> LoginAsync(LoginDto loginDto);
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<bool> DeleteUser(Guid id);
        Task<bool> DeActivateUser(Guid id);
        Task<IEnumerable<UserWithPharmacyDto>> GetUsersByRoleWithPharmacyAsync(Guid roleId);
        Task<UserDto> GetUser(Guid id);
    }
}
