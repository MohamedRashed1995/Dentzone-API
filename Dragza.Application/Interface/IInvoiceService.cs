using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> GenerateInvoiceForOrderAsync(Guid orderId);
        Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId);
        Task<InvoiceDto> GetInvoiceByOrderIdAsync(Guid orderId);
        Task<IEnumerable<InvoiceDto>> GetUserInvoicesAsync(Guid userId);
        Task<IEnumerable<InvoiceDto>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InvoiceDto>> GetInventoryInvoicesAsync(Guid userId);
    }
}
