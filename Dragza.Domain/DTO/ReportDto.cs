

namespace Dragza.Domain.DTO;

public class InvoiceReportDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string InvoiceType { get; set; }
    public Guid OrderId { get; set; }
    public Guid PharmacyUserId { get; set; }
    public string PharmacyName { get; set; }
    public decimal CreditUsed { get; set; }
    public decimal CashPaid { get; set; }
    public string PaymentMethod { get; set; }
}

public class OrderReportDto
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? DeliverDate { get; set; }
    public Guid PharmacyUserId { get; set; }
    public string PharmacyName { get; set; }
    public int ItemCount { get; set; }
    public decimal CreditUsed { get; set; }
    public decimal CashPaid { get; set; }
    public string PaymentMethod { get; set; }
    public bool HasInvoice { get; set; }
}

public class SalesSummaryDto
{
    public decimal TotalSales { get; set; }
    public decimal TotalCash { get; set; }
    public decimal TotalCredit { get; set; }
    public int TotalOrders { get; set; }
    public int TotalInvoices { get; set; }
}

public class ReportFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? PharmacyUserId { get; set; }
    public Guid? InventoryUserId { get; set; }  // Added this line
    public Guid? RegionId { get; set; }
    public int? Status { get; set; }
    public string? PaymentMethod { get; set; } // "cash", "credit", "mixed"
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
