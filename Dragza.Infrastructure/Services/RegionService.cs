using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;

namespace Dragza.Infrastructure.Services
{
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid?> GetRegionIdAsync(string regionName, string districtName, Guid? governateId, Guid? cityId)
        {
            if (!string.IsNullOrEmpty(regionName))
            {
                var region = await _unitOfWork.RegionRepository
                    .FindAsync(r => r.RegionName == regionName);
                return region.FirstOrDefault()?.Id;
            }

            if (governateId.HasValue)
            {
                var governate = await _unitOfWork.GovernateRepository.GetByIdAsync(governateId.Value);
                return governate?.RegionId;
            }

            if (cityId.HasValue)
            {
                var city = await _unitOfWork.CityRepository.GetByIdAsync(cityId.Value);
                var governate = await _unitOfWork.GovernateRepository.GetByIdAsync(city.GovernateId);
                return governate?.RegionId;
            }

            return null;
        }

        public async Task<RegionWithUsersDto> GetByIdAsync(Guid id)
        {
            var region = await _unitOfWork.RegionRepository.GetByIdAsync(id);
            return _mapper.Map<RegionWithUsersDto>(region);
        }

        public async Task<IEnumerable<RegionDto>> GetAllAsync(bool isActive)
        {
            if (isActive)
            {
                var regions = await _unitOfWork.RegionRepository.GetAllAsync(a => a.IsDeleted != true);
                return _mapper.Map<IEnumerable<RegionDto>>(regions);
            }
            else
            {
                var regions = await _unitOfWork.RegionRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<RegionDto>>(regions);
            }
        }

        public async Task<RegionDto> CreateAsync(CreateRegionDto createDto)
        {
            var region = _mapper.Map<Region>(createDto);
            await _unitOfWork.RegionRepository.AddAsync(region);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RegionDto>(region);
        }

        public async Task UpdateAsync(Guid id, UpdateRegionDto updateDto)
        {
            var existing = await _unitOfWork.RegionRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Region not found");

            _mapper.Map(updateDto, existing);
            _unitOfWork.RegionRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _unitOfWork.RegionRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Region not found");
            _unitOfWork.RegionRepository.Delete(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRegionAsync(Guid regionId)
        {
            var users = await _unitOfWork.RegionRepository.GetUsersByRegionAsync(regionId);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<bool> ChangeStatus(Guid id)
        {
            var region = await _unitOfWork.RegionRepository.GetByIdAsync(id);
            if (region == null)
                throw new KeyNotFoundException("region not found");
            if (region.IsDeleted == null)
                region.IsDeleted = true; // Toggle the status
            else
                region.IsDeleted = !region.IsDeleted; // Toggle the status

            _unitOfWork.RegionRepository.Update(region);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
