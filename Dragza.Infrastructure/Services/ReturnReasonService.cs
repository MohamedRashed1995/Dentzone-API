using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class ReturnReasonService : IReturnReasonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReturnReasonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReturnReasonDto> GetByIdAsync(Guid id)
        {
            var reason = await _unitOfWork.ReturnReasonRepository.GetByIdAsync(id);
            if (reason == null)
            {
                throw new KeyNotFoundException($"Return reason with ID {id} not found");
            }
            return _mapper.Map<ReturnReasonDto>(reason);
        }

        public async Task<IEnumerable<ReturnReasonDto>> GetAllAsync()
        {
            var reasons = await _unitOfWork.ReturnReasonRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReturnReasonDto>>(reasons);
        }

        public async Task<ReturnReasonDto> CreateAsync(CreateReturnReasonDto createDto)
        {

            var reason = _mapper.Map<ReturnReason>(createDto);
            _unitOfWork.ReturnReasonRepository.AddAsync(reason);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ReturnReasonDto>(reason);
        }

        public async Task UpdateAsync(Guid id, UpdateReturnReasonDto updateDto)
        {
            var existing = await _unitOfWork.ReturnReasonRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Return reason with ID {id} not found");
            }


            _mapper.Map(updateDto, existing);
             _unitOfWork.ReturnReasonRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _unitOfWork.ReturnReasonRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Return reason with ID {id} not found");
            }

            if (existing.ReturnedItems.Any())
            {
                throw new InvalidOperationException("Cannot delete return reason that has associated returned items");
            }

             _unitOfWork.ReturnReasonRepository.Delete(existing);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
