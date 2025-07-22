using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IBalanceReportingService
    {
        Task<PaginatedResult<BalanceAccountReportDto>> GetBalanceAccountsReportAsync(BalanceReportFilterDto filter);
        Task<PaginatedResult<BalanceTransactionReportDto>> GetBalanceTransactionsReportAsync(BalanceReportFilterDto filter);
        Task<BalanceSummaryDto> GetBalanceSummaryAsync(BalanceReportFilterDto filter);
    }
}
