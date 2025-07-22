// IGovernateService.cs
using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;

namespace Dragza.Infrastructure.Services;

public class GovernateService : IGovernateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GovernateService(
         IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GovernateWithCitiesDto> GetByIdAsync(Guid id)
    {
        var governate = await _unitOfWork.GovernateRepository.GetByIdAsync(id);
        return _mapper.Map<GovernateWithCitiesDto>(governate);
    }

    public async Task<IEnumerable<GovernateDto>> GetAllAsync(bool isActive)
    {
        if (isActive)
        {
            var governates = await _unitOfWork.GovernateRepository.GetAllAsync(a => a.IsDeleted != true);
            return _mapper.Map<IEnumerable<GovernateDto>>(governates);
        }
        else
        {
            var governates = await _unitOfWork.GovernateRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GovernateDto>>(governates);
        }

    }

    public async Task<GovernateDto> CreateAsync(CreateGovernateDto createDto)
    {
        if (createDto.RegionId.HasValue && !await _unitOfWork.RegionRepository.ExistsAsync(createDto.RegionId.Value))
        {
            throw new KeyNotFoundException("Region not found");
        }

        var governate = _mapper.Map<Governate>(createDto);
        await _unitOfWork.GovernateRepository.AddAsync(governate);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GovernateDto>(governate);

    }

    public async Task UpdateAsync(Guid id, UpdateGovernateDto updateDto)
    {
        var existing = await _unitOfWork.GovernateRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException("Governate not found");

        if (updateDto.RegionId.HasValue && !await _unitOfWork.RegionRepository.ExistsAsync(updateDto.RegionId.Value))
        {
            throw new KeyNotFoundException("Region not found");
        }

        _mapper.Map(updateDto, existing);
         _unitOfWork.GovernateRepository.Update(existing);
        await _unitOfWork.SaveChangesAsync();

    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _unitOfWork.GovernateRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException("Governate not found");
        _unitOfWork.GovernateRepository.Delete(existing);
        await _unitOfWork.SaveChangesAsync();

    }
    public async Task<List<GovernateDto>> GetAllWithRegionId(Guid regionId)
    {
        var governates = await _unitOfWork.GovernateRepository.GetAllAsync();
        var filteredGovernates = governates.Where(g => g.RegionId == regionId).ToList();
        return _mapper.Map<List<GovernateDto>>(filteredGovernates);
    }
    public async Task<bool> ChangeStatus(Guid id)
    {
        var governate = await _unitOfWork.GovernateRepository.GetByIdAsync(id);
        if (governate == null)
            throw new KeyNotFoundException("Governate not found");

        await _unitOfWork.GovernateRepository.ChangeStatus(id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
