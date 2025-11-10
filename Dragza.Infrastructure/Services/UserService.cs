using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Dragza.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;

        public UserService(
            IUnitOfWork unitOfWork,
            PasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IMapper mapper, IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
            _fileStorage = fileStorage;

        }

        public async Task<UserDto> GetUser(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            var dto = _mapper.Map<UserDto>(user);
            //if (dto != null && user.IsPharmacy == true)
            //{
            //    dto.Accountid = user.BalanceAccounts.FirstOrDefault().Id;
            //}
            return dto;
        }
        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                user.DeletedDate = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            try
            {
                var users = (await _unitOfWork.UserRepository.GetAllAsync(
                        include: q => q.Include(u => u.Region)
                       )).ToList();

                if (users == null || !users.Any())
                    return new List<UserDto>();

                // Test with single user first for debugging
                //var testDto = _mapper.Map<UserDto>(users.First());

                return _mapper.Map<List<UserDto>>(users);

            }
            catch (Exception ex)
            {
                return new List<UserDto>();
            }
        }

        public async Task<JWTTokenDTO> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.UserRepository.FindByUsernameOrEmailAsync(loginDto.UsernameOrEmail);
            if (user.IsDeleted == true)
            {
               return new JWTTokenDTO { HasDetails = false };
            }


            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var token = _jwtTokenGenerator.GenerateToken(user);
            return new JWTTokenDTO
            {
                Token = token,
                HasDetails = true,
                IsEmailVerified = (bool)user.EmailConfirmed,
                IsPhoneVerified = (bool)user.PhoneConfirmed,
                UserId = user.Id.ToString(),
                Role = user.UserRoles.FirstOrDefault().Role.Name
            };
        }
        public async Task<UserResponseDto> RegisterUserAsync(CreateUserDto createUserDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingUser = await _unitOfWork.UserRepository.FindByUsernameOrEmailAsync(createUserDto.Email);
                if (existingUser != null)
                    throw new ApplicationException("Username or email already exists");

                var user = _mapper.Map<User>(createUserDto);
                user.Id = Guid.NewGuid();
                user.Password = _passwordHasher.HashPassword(createUserDto.Password);
                user.IsActive = true; // Default to active
                user.IsDeleted = false;
                if((createUserDto.GovId != null 
                    && createUserDto.GovId != Guid.Empty) 
                    &&( createUserDto.RegionId == null
                    ||createUserDto.RegionId == Guid.Empty))
                {
                    user.RegionId = createUserDto.GovId;
                }
                else
                {
                    user.RegionId = createUserDto.RegionId;
                }

                user.CreatedAt = DateTime.UtcNow;
                if (createUserDto.Photo != null)
                {
                    user.Photo = await _fileStorage.SaveFileAsync(
                           createUserDto.Photo);
                }
                // Handle pharmacy details
                if (createUserDto.IsPharmacy)
                {
                    if (createUserDto.PharmacyDetails != null)
                    {

                        var pharmacyDetails = _mapper.Map<PharmacyDetail>(createUserDto.PharmacyDetails);

                        pharmacyDetails.Id = Guid.NewGuid();
                        pharmacyDetails.UserId = user.Id;
                        pharmacyDetails.PurchasingManager = user.Id;

                        if (createUserDto.PharmacyDetails.CommercialRegisteryAttach != null)
                        {

                            pharmacyDetails.CommercialRegisteryAttach = await _fileStorage.SaveFileAsync(
                                createUserDto.PharmacyDetails.CommercialRegisteryAttach);
                        }

                        if (createUserDto.PharmacyDetails.NationalIdAttach != null)
                        {
                            pharmacyDetails.NationalIdAttach = await _fileStorage.SaveFileAsync(
        createUserDto.PharmacyDetails.NationalIdAttach);
                        }

                        if (createUserDto.PharmacyDetails.TaxationCardAttach != null)
                        {
                            pharmacyDetails.TaxationCardAttach = await _fileStorage.SaveFileAsync(
       createUserDto.PharmacyDetails.TaxationCardAttach);
                        }

                        if (createUserDto.PharmacyDetails.OwnersgipAttach != null)
                        {
                            pharmacyDetails.OwnersgipAttach = await _fileStorage.SaveFileAsync(
      createUserDto.PharmacyDetails.OwnersgipAttach);
                        }

                        if (createUserDto.PharmacyDetails.PharmacyLicenseAttach != null)
                        {
                            pharmacyDetails.PharmacyLicenseAttach = await _fileStorage.SaveFileAsync(
      createUserDto.PharmacyDetails.PharmacyLicenseAttach);
                        }


                        await _unitOfWork.PharmacyDetailRepository.AddAsync(pharmacyDetails);
                    }
                }

                // Assign role
                var role = await _unitOfWork.RoleRepository
                    .FindAsync(r => r.Id == createUserDto.RoleId);

                if (role == null)
                    throw new ApplicationException("Default role not found");

                user.UserRoles.Add(new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    RoleId = role.FirstOrDefault().Id
                });

                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<UserResponseDto>(user);

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }


        }

        public async Task<IEnumerable<UserWithPharmacyDto>> GetUsersByRoleWithPharmacyAsync(Guid roleId)
        {
            // Verify role exists
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
            if (role == null) throw new Exception("Role not found");

            var users = await _unitOfWork.UserRepository.GetUsersByRoleWithPharmacyAsync(roleId);

            return _mapper.Map<IEnumerable<UserWithPharmacyDto>>(users);
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

        public async Task<UserResponseDto> UpdateUserAsync(Guid userid ,UpdateUserDto createUserDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(userid);
                
                _mapper.Map(createUserDto, existingUser);
                if (createUserDto.Password != null)
                        existingUser.Password = _passwordHasher.HashPassword(createUserDto.Password);

                if (createUserDto.Photo != null)
                {
                    existingUser.Photo = await _fileStorage.SaveFileAsync(
                           createUserDto.Photo);
                }
                // Handle pharmacy details
                if (createUserDto.IsPharmacy)
                {
                    if (existingUser.PharmacyDetailUsers != null)
                    {
                        var existingPharmacy = existingUser.PharmacyDetailUsers.FirstOrDefault();

                        _mapper.Map(createUserDto.PharmacyDetails, existingPharmacy);


                        if (createUserDto.PharmacyDetails.CommercialRegisteryAttach != null)
                        {

                            existingPharmacy.CommercialRegisteryAttach = await _fileStorage.SaveFileAsync(
                                createUserDto.PharmacyDetails.CommercialRegisteryAttach);
                        }

                        if (createUserDto.PharmacyDetails.NationalIdAttach != null)
                        {
                            existingPharmacy.NationalIdAttach = await _fileStorage.SaveFileAsync(
        createUserDto.PharmacyDetails.NationalIdAttach);
                        }

                        if (createUserDto.PharmacyDetails.TaxationCardAttach != null)
                        {
                            existingPharmacy.TaxationCardAttach = await _fileStorage.SaveFileAsync(
       createUserDto.PharmacyDetails.TaxationCardAttach);
                        }

                        if (createUserDto.PharmacyDetails.OwnersgipAttach != null)
                        {
                            existingPharmacy.OwnersgipAttach = await _fileStorage.SaveFileAsync(
      createUserDto.PharmacyDetails.OwnersgipAttach);
                        }

                        if (createUserDto.PharmacyDetails.PharmacyLicenseAttach != null)
                        {
                            existingPharmacy.PharmacyLicenseAttach = await _fileStorage.SaveFileAsync(
      createUserDto.PharmacyDetails.PharmacyLicenseAttach);
                        }


                         _unitOfWork.PharmacyDetailRepository.Update(existingPharmacy);
                    }
                }


                _unitOfWork.UserRepository.Update(existingUser);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<UserResponseDto>(existingUser);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }


        }

        public async Task<bool> ChangePasswordAsync(ChangePassword model)
        {
                using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId);

                var oldPassword = _passwordHasher.HashPassword(model.CurrentPassword);

                var isRight = _passwordHasher.VerifyPassword(model.CurrentPassword, oldPassword);

                if (oldPassword==existingUser.Password)
                {
                    existingUser.Password = _passwordHasher.HashPassword(model.NewPassword);
                    _unitOfWork.UserRepository.Update(existingUser);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    return false;
                }
              
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                throw;
            }
           


        }

       

    }
}
