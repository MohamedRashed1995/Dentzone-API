using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{

    public class BalanceReportingService : IBalanceReportingService
    {
        private readonly IBalanceReportingRepository _repository;
        private readonly IMapper _mapper;

        public BalanceReportingService(IBalanceReportingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<BalanceAccountReportDto>> GetBalanceAccountsReportAsync(BalanceReportFilterDto filter)
        {
            var accounts = await _repository.GetBalanceAccountsReportAsync(filter);
            var totalCount = await _repository.GetBalanceAccountsCountAsync(filter);

            var accountDtos = _mapper.Map<IEnumerable<BalanceAccountReportDto>>(accounts);

            return new PaginatedResult<BalanceAccountReportDto>(
                accountDtos,
                totalCount,
                filter.PageNumber,
                filter.PageSize);
        }

        public async Task<PaginatedResult<BalanceTransactionReportDto>> GetBalanceTransactionsReportAsync(BalanceReportFilterDto filter)
        {
            var transactions = await _repository.GetBalanceTransactionsReportAsync(filter);
            var totalCount = await _repository.GetBalanceTransactionsCountAsync(filter);

            var transactionDtos = _mapper.Map<IEnumerable<BalanceTransactionReportDto>>(transactions);

            return new PaginatedResult<BalanceTransactionReportDto>(
                transactionDtos,
                totalCount,
                filter.PageNumber,
                filter.PageSize);
        }

        public async Task<BalanceSummaryDto> GetBalanceSummaryAsync(BalanceReportFilterDto filter)
        {
            return await _repository.GetBalanceSummaryAsync(filter);
        }
    }
}
