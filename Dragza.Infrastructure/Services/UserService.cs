using AutoMapper;
using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Dragza.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly DragzaContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        public UserService(
            DragzaContext context,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IMapper mapper, IFileStorageService fileStorage,
            ILogger<UserService> logger,
            IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _logger = logger;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<UserDto> GetUser(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            var dto = _mapper.Map<UserDto>(user);
            
            return dto;
        }
        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

        //public async Task<List<UserDto>> GetAllUsers()
        //{
        //    try
        //    {
        //        var users = (await _userRepository.GetAllAsync());

        //        if (users == null || !users.Any())
        //            return new List<UserDto>();

        //        return users.Select(u => new UserDto
        //        {
        //            Id = u.Id,
        //            FullName = u.FullName,
        //            Email = u.Email,
        //            PhoneNumber = u.PhoneNumber,
        //            IsActive = u.IsActive,
        //            IsDeleted = u.IsDeleted,
        //            CreatedAt = u.CreatedAt, 
        //            Addresses = u.Addresses.Select(a => new Address
        //            {
        //                Id = a.Id,
        //                UserId = a.UserId,
        //                AddressLine = a.AddressLine
        //            }).ToList(),
        //            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList() // هنا بجيب كل الـ roles
        //        }).ToList();

        //        //return _mapper.Map<List<UserDto>>(users);

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        throw;
        //    }

        //}
        public async Task<List<UserDto>> GetAllUsers()
        {
            try
            {
                return await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Addresses)
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        IsActive = u.IsActive,
                        IsDeleted = u.IsDeleted,
                        CreatedAt = u.CreatedAt,

                        Addresses = u.Addresses.Select(a => new AddressResponseDto
                        {
                            Id = a.Id,
                            UserId = a.UserId,
                            AddressLine = a.AddressLine
                        }).ToList(),

                        //Roles = u.UserRoles
                        //    .Select(ur => ur.Role.Name)
                        //    .ToList()
                        Roles = u.UserRoles
                            .Select(ur => new RoleDto
                            {
                                Id = ur.Role.Id,
                                Name = ur.Role.Name
                            })
                            .ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .Include(u => u.Addresses) // <--- ده اللي محتاجه
                        .FirstOrDefaultAsync(u => u.Email == loginDto.UsernameOrEmail);
               
                if (user == null || user.IsDeleted)
                {
                    _logger.LogWarning("Login failed: user not found");
                    return new LoginResponseDto { HasDetails = false };
                }

                if (!_passwordHasher.VerifyPassword(loginDto.Password, user.Password))
                {
                    _logger.LogWarning("Login failed: invalid password for user {Email}", loginDto.UsernameOrEmail);
                    return new LoginResponseDto { HasDetails = false };
                }

                var token = _jwtTokenGenerator.GenerateToken(user);

                return new LoginResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    Token = token,
                    HasDetails = true,
                    Role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "",
                    Addresses = user.Addresses.Select(a => new AddressResponseDto
                    {
                        Id = a.Id,
                        UserId = a.UserId,
                        AddressLine = a.AddressLine
                    }).ToList() 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login exception for {Email}", loginDto.UsernameOrEmail);
                throw;
            }
        }



        public async Task<UserResponseDto> RegisterUserAsync(CreateUserDto createUserDto)
        {
            var strategy = _unitOfWork.DbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.DbContext.Database.BeginTransactionAsync();
                try
                {
                    var existingByEmail = await _unitOfWork.UserRepository
                        .FindByUsernameOrEmailAsync(createUserDto.Email);

                    if (existingByEmail != null)
                        throw new ApplicationException("Email already exists");

                    if (!createUserDto.RoleId.HasValue || createUserDto.RoleId == Guid.Empty)
                    {
                        createUserDto.RoleId = Guid.Parse("e48e5a9f-2074-4de9-a849-5c69fdd45e4e");
                    }

                    var user = _mapper.Map<User>(createUserDto);
                    user.Id = Guid.NewGuid();
                    user.Password = _passwordHasher.HashPassword(createUserDto.Password);
                    user.IsDeleted = false;
                    user.CreatedAt = DateTime.UtcNow;

                    if (createUserDto.AddressLines != null && createUserDto.AddressLines.Any())
                    {
                        foreach (var line in createUserDto.AddressLines)
                        {
                            user.Addresses.Add(new Address
                            {
                                Id = Guid.NewGuid(),
                                UserId = user.Id,
                                AddressLine = line
                            });
                        }
                    }

                    var role = await _unitOfWork.RoleRepository.GetByIdAsync(createUserDto.RoleId);
                    if (role == null)
                        throw new ApplicationException($"Role with ID '{createUserDto.RoleId}' not found");

                    user.UserRoles.Add(new UserRole
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        RoleId = role.Id
                    });

                    await _unitOfWork.UserRepository.AddAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // بدل الـ mapper، نعمل DTO يدويًا نظيف
                    var userDto = new UserResponseDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        AddressLines = user.Addresses
                            .Where(a => !string.IsNullOrEmpty(a.AddressLine))
                            .Select(a => a.AddressLine!)
                            .ToList()
                    };

                    return userDto;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }



        public async Task<List<UserWithSpecificRolesDto>> GetUsersByRoleWithPharmacyAsync(Guid roleId)
        {
            // Verify role exists
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
            if (role == null) throw new Exception("Role not found");

            var users = await _unitOfWork.UserRepository.GetUsersByRoleWithPharmacyAsync(roleId);

            return _mapper.Map<List<UserWithSpecificRolesDto>>(users);
        }

        public async Task<bool> DeActivateUser(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user != null && user.IsActive == true)
            {
                user.IsActive = false;
                 _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.CommitAsync();
            }else if (user != null && user.IsActive == false)
            {
                user.IsActive = true;
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.CommitAsync();
            }
            else if(user != null && user.IsActive == null)
            {
                user.IsActive = false;
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.CommitAsync();
            }
            return false;
        }

        public async Task<UserResponseDto> UpdateUserAsync(Guid userid, UpdateUserDto updateUserDto)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(userid);

            if (existingUser == null)
                return null;

            _mapper.Map(updateUserDto, existingUser);

            //if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
            //{
            //    existingUser.Password = _passwordHasher.HashPassword(updateUserDto.Password);
            //}

            _unitOfWork.UserRepository.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserResponseDto>(existingUser);
        }




        public async Task<bool> ChangePasswordAsync(ChangePassword model)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId);

            if (existingUser == null)
                return false;

            if (string.IsNullOrEmpty(existingUser.Password))
                throw new Exception("Stored password is null");

            var isRight = _passwordHasher.VerifyPassword(
                model.CurrentPassword,
                existingUser.Password
            );

            if (!isRight)
                return false;

            existingUser.Password = _passwordHasher.HashPassword(model.NewPassword);

            _unitOfWork.UserRepository.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }



    }
}
