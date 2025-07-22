using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IBalanceReportingRepository
    {
        Task<IEnumerable<BalanceAccount>> GetBalanceAccountsReportAsync(BalanceReportFilterDto filter);
        Task<IEnumerable<BalanceTransaction>> GetBalanceTransactionsReportAsync(BalanceReportFilterDto filter);
        Task<BalanceSummaryDto> GetBalanceSummaryAsync(BalanceReportFilterDto filter);
        Task<int> GetBalanceAccountsCountAsync(BalanceReportFilterDto filter);
        Task<int> GetBalanceTransactionsCountAsync(BalanceReportFilterDto filter);
    }
}
