// InvoiceService.cs
using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System.Net.Mail;

namespace Dragza.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InvoiceService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
       
    }

    public async Task<InvoiceDto> GenerateInvoiceForOrderAsync(Guid orderId)
    {
        // Check if invoice already exists
        if (await _unitOfWork.InvoiceRepository.ExistsForOrderAsync(orderId))
        {
            throw new InvalidOperationException("Invoice already exists for this order");
        }

        // Get order details
        var order = await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(orderId);
        if (order == null)
        {
            throw new KeyNotFoundException("Order not found");
        }

        // Determine invoice type based on payment method
        var invoiceType = order.CreditUsed > 0
            ? order.CashPaid > 0
                ? await _unitOfWork.InvoiceTypeRepository.GetByNameAsync("Mixed")
                : await _unitOfWork.InvoiceTypeRepository.GetByNameAsync("Credit")
            : await _unitOfWork.InvoiceTypeRepository.GetByNameAsync("Cash");

        if (invoiceType == null)
        {
            throw new InvalidOperationException("Invoice type not configured");
        }

        // Create invoice
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),

            OrderId = order.Id,
            InvoiceDate = DateTime.UtcNow,
            TotalAmount = order.TotalAmount,
            PharmacyUserId = order.PharmacyUserId,
            InvoiceTypeId = invoiceType.Id
        };

        // Save invoice
        await _unitOfWork.InvoiceRepository.AddAsync(invoice);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId)
    {
        var invoice = await _unitOfWork.InvoiceRepository.GetByIdAsync(invoiceId);
        if (invoice == null)
        {
            throw new KeyNotFoundException("Invoice not found");
        }

        return _mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<InvoiceDto> GetInvoiceByOrderIdAsync(Guid orderId)
    {
        var invoice = await _unitOfWork.InvoiceRepository.GetByOrderIdAsync(orderId);
        if (invoice == null)
        {
            throw new KeyNotFoundException("Invoice not found for this order");
        }

        return _mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<IEnumerable<InvoiceDto>> GetUserInvoicesAsync(Guid userId)
    {
        var invoices = await _unitOfWork.InvoiceRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
    }

    public async Task<IEnumerable<InvoiceDto>> GetInventoryInvoicesAsync(Guid userId)
    {
        var invoices = await _unitOfWork.InvoiceRepository.GetByInvintoryIdAsync(userId);
        return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
    }
    public async Task<IEnumerable<InvoiceDto>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var invoices = await _unitOfWork.InvoiceRepository.GetByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
    }

}