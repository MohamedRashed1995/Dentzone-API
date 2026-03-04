// ReportingRepository.cs
using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore;

namespace Dragza.Infrastructure.Services;
public class ReportingRepository : IReportingRepository
{
    private readonly DragzaContext _context;

    public ReportingRepository(DragzaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesReportAsync(ReportFilterDto filter)
    {
        var query = _context.Invoices
        .Include(i => i.InvoiceType)
        .Include(i => i.Order)
            .ThenInclude(o => o.User)
        .Include(i => i.Order)
            .ThenInclude(o => o.InventoryUserId)  // Added this include
        .AsQueryable();

        query = ApplyCommonFilters(query, filter);
        query = ApplyInvoiceSpecificFilters(query, filter);

        return await query
            .OrderByDescending(i => i.InvoiceDate)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersReportAsync(ReportFilterDto filter)
    {
        var query = _context.Orders
         .Include(o => o.User)
         .Include(o => o.InventoryUserId)  // Added this include
         .Include(o => o.OrderItems)
         .Include(o => o.Invoices)
         .AsQueryable();

        query = ApplyCommonFilters(query, filter);
        query = ApplyOrderSpecificFilters(query, filter);

        return await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }

    public async Task<SalesSummaryDto> GetSalesSummaryAsync(ReportFilterDto filter)
    {
        var invoiceQuery = _context.Invoices.AsQueryable();
        var orderQuery = _context.Orders.AsQueryable();

        invoiceQuery = ApplyCommonFilters(invoiceQuery, filter);
        orderQuery = ApplyCommonFilters(orderQuery, filter);

        var totalInvoices = await invoiceQuery.CountAsync();
        var totalOrders = await orderQuery.CountAsync();

        var totalSales = await invoiceQuery.SumAsync(i => i.TotalAmount);
        var totalCash = await orderQuery.SumAsync(o => o.CashPaid ?? 0);
        var totalCredit = await orderQuery.SumAsync(o => o.CreditUsed ?? 0);

        return new SalesSummaryDto
        {
            TotalSales = totalSales,
            TotalCash = totalCash,
            TotalCredit = totalCredit,
            TotalOrders = totalOrders,
            TotalInvoices = totalInvoices
        };
    }

    public async Task<int> GetTotalCountAsync<T>(ReportFilterDto filter) where T : class
    {
        var query = _context.Set<T>().AsQueryable();

        if (typeof(T) == typeof(Invoice))
        {
            var invoiceQuery = (IQueryable<Invoice>)query;
            invoiceQuery = ApplyCommonFilters(invoiceQuery, filter);
            invoiceQuery = ApplyInvoiceSpecificFilters(invoiceQuery, filter);
            return await invoiceQuery.CountAsync();
        }
        else if (typeof(T) == typeof(Order))
        {
            var orderQuery = (IQueryable<Order>)query;
            orderQuery = ApplyCommonFilters(orderQuery, filter);
            orderQuery = ApplyOrderSpecificFilters(orderQuery, filter);
            return await orderQuery.CountAsync();
        }

        return await query.CountAsync();
    }

    private IQueryable<Invoice> ApplyInvoiceSpecificFilters(IQueryable<Invoice> query, ReportFilterDto filter)
    {
        if (!string.IsNullOrEmpty(filter.PaymentMethod))
        {
            query = filter.PaymentMethod.ToLower() switch
            {
                "cash" => query.Where(i => i.Order.CashPaid > 0 && (i.Order.CreditUsed == null || i.Order.CreditUsed == 0)),
                "credit" => query.Where(i => i.Order.CreditUsed > 0 && (i.Order.CashPaid == null || i.Order.CashPaid == 0)),
                "mixed" => query.Where(i => i.Order.CreditUsed > 0 && i.Order.CashPaid > 0),
                _ => query
            };
        }

        return query;
    }

    private IQueryable<Order> ApplyOrderSpecificFilters(IQueryable<Order> query, ReportFilterDto filter)
    {
        if (filter.Status.HasValue)
        {
            query = query.Where(o => o.Status == filter.Status.Value);
        }

        if (!string.IsNullOrEmpty(filter.PaymentMethod))
        {
            query = filter.PaymentMethod.ToLower() switch
            {
                "cash" => query.Where(o => o.CashPaid > 0 && (o.CreditUsed == null || o.CreditUsed == 0)),
                "credit" => query.Where(o => o.CreditUsed > 0 && (o.CashPaid == null || o.CashPaid == 0)),
                "mixed" => query.Where(o => o.CreditUsed > 0 && o.CashPaid > 0),
                _ => query
            };
        }

        return query;
    }

    private IQueryable<T> ApplyCommonFilters<T>(IQueryable<T> query, ReportFilterDto filter) where T : class
    {
        if (filter.StartDate.HasValue)
        {
            if (typeof(T) == typeof(Invoice))
            {
                query = (IQueryable<T>)((IQueryable<Invoice>)query)
                    .Where(i => i.InvoiceDate >= filter.StartDate.Value);
            }
            else if (typeof(T) == typeof(Order))
            {
                query = (IQueryable<T>)((IQueryable<Order>)query)
                    .Where(o => o.OrderDate >= filter.StartDate.Value);
            }
        }

        if (filter.EndDate.HasValue)
        {
            if (typeof(T) == typeof(Invoice))
            {
                query = (IQueryable<T>)((IQueryable<Invoice>)query)
                    .Where(i => i.InvoiceDate <= filter.EndDate.Value);
            }
            else if (typeof(T) == typeof(Order))
            {
                query = (IQueryable<T>)((IQueryable<Order>)query)
                    .Where(o => o.OrderDate <= filter.EndDate.Value);
            }
        }

        if (filter.UserId.HasValue)
        {
            if (typeof(T) == typeof(Invoice))
            {
                query = (IQueryable<T>)((IQueryable<Invoice>)query)
                    .Where(i => i.UserId == filter.UserId.Value);
            }
            else if (typeof(T) == typeof(Order))
            {
                query = (IQueryable<T>)((IQueryable<Order>)query)
                    .Where(o => o.UserId == filter.UserId.Value);
            }
        }

        if (filter.InventoryUserId.HasValue)  // Added this filter
        {
            if (typeof(T) == typeof(Invoice))
            {
                query = (IQueryable<T>)((IQueryable<Invoice>)query)
                    .Where(i => i.Order.InventoryUserId == filter.InventoryUserId.Value);
            }
            else if (typeof(T) == typeof(Order))
            {
                query = (IQueryable<T>)((IQueryable<Order>)query)
                    .Where(o => o.InventoryUserId == filter.InventoryUserId.Value);
            }
        }

        

        return query;
    }
}