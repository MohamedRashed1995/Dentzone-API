using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IReportingRepository
    {
        Task<IEnumerable<Invoice>> GetInvoicesReportAsync(ReportFilterDto filter);
        Task<IEnumerable<Order>> GetOrdersReportAsync(ReportFilterDto filter);
        Task<SalesSummaryDto> GetSalesSummaryAsync(ReportFilterDto filter);
        Task<int> GetTotalCountAsync<T>(ReportFilterDto filter) where T : class;
    }
}
