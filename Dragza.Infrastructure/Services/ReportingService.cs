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
    public class ReportingService : IReportingService
    {
        private readonly IReportingRepository _reportingRepository;
        private readonly IMapper _mapper;

        public ReportingService(IReportingRepository reportingRepository, IMapper mapper)
        {
            _reportingRepository = reportingRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<InvoiceReportDto>> GetInvoicesReportAsync(ReportFilterDto filter)
        {
            var invoices = await _reportingRepository.GetInvoicesReportAsync(filter);
            var totalCount = await _reportingRepository.GetTotalCountAsync<Invoice>(filter);

            var invoiceDtos = _mapper.Map<IEnumerable<InvoiceReportDto>>(invoices);

            return new PaginatedResult<InvoiceReportDto>(
                invoiceDtos,
                totalCount,
                filter.PageNumber,
                filter.PageSize);
        }

        public async Task<PaginatedResult<OrderReportDto>> GetOrdersReportAsync(ReportFilterDto filter)
        {
            var orders = await _reportingRepository.GetOrdersReportAsync(filter);
            var totalCount = await _reportingRepository.GetTotalCountAsync<Order>(filter);

            var orderDtos = _mapper.Map<IEnumerable<OrderReportDto>>(orders);

            return new PaginatedResult<OrderReportDto>(
                orderDtos,
                totalCount,
                filter.PageNumber,
                filter.PageSize);
        }

        public async Task<SalesSummaryDto> GetSalesSummaryAsync(ReportFilterDto filter)
        {
            return await _reportingRepository.GetSalesSummaryAsync(filter);
        }
    }
}
