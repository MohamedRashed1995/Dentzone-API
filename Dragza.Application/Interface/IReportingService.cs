using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IReportingService
    {
        Task<PaginatedResult<InvoiceReportDto>> GetInvoicesReportAsync(ReportFilterDto filter);
        Task<PaginatedResult<OrderReportDto>> GetOrdersReportAsync(ReportFilterDto filter);
        Task<SalesSummaryDto> GetSalesSummaryAsync(ReportFilterDto filter);
    }
}
