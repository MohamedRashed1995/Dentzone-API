// ReportsController.cs
using ClosedXML.Excel;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Dragza.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin,InventoryManager")]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportsController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet("invoices")]
    public async Task<ActionResult<PaginatedResult<InvoiceReportDto>>> GetInvoicesReport(
        [FromQuery] ReportFilterDto filter)
    {
        var result = await _reportingService.GetInvoicesReportAsync(filter);
        return Ok(result);
    }

    [HttpGet("orders")]
    public async Task<ActionResult<PaginatedResult<OrderReportDto>>> GetOrdersReport(
        [FromQuery] ReportFilterDto filter)
    {
        var result = await _reportingService.GetOrdersReportAsync(filter);
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<SalesSummaryDto>> GetSalesSummary(
        [FromQuery] ReportFilterDto filter)
    {
        var result = await _reportingService.GetSalesSummaryAsync(filter);
        return Ok(result);
    }

    [HttpGet("export/invoices")]
    public async Task<IActionResult> ExportInvoicesReport(
      [FromQuery] ReportFilterDto filter)
    {
        filter.PageNumber = 1;
        filter.PageSize = int.MaxValue; // Get all records

        var result = await _reportingService.GetInvoicesReportAsync(filter);

        // Create a new Excel workbook
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Invoices");

        // Add headers
        var headers = new string[] {
        "InvoiceNumber",
        "InvoiceDate",
        "TotalAmount",
        "PharmacyName",
        "PaymentMethod",
        "CreditUsed",
        "CashPaid"
    };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        // Add data rows
        int currentRow = 2;
        foreach (var invoice in result.Items)
        {
            worksheet.Cell(currentRow, 1).Value = invoice.InvoiceNumber;
            worksheet.Cell(currentRow, 2).Value = invoice.InvoiceDate;
            worksheet.Cell(currentRow, 2).Style.DateFormat.Format = "yyyy-MM-dd";
            worksheet.Cell(currentRow, 3).Value = invoice.TotalAmount;
            worksheet.Cell(currentRow, 4).Value = invoice.PharmacyName;
            worksheet.Cell(currentRow, 5).Value = invoice.PaymentMethod;
            worksheet.Cell(currentRow, 6).Value = invoice.CreditUsed;
            worksheet.Cell(currentRow, 7).Value = invoice.CashPaid;
            currentRow++;
        }

        // Format the header row
        var headerRange = worksheet.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Format currency columns
        var currencyColumns = new int[] { 3, 6, 7 }; // TotalAmount, CreditUsed, CashPaid
        foreach (var col in currencyColumns)
        {
            worksheet.Column(col).Style.NumberFormat.Format = "#,##0.00";
        }

        // Auto-fit columns for better display
        worksheet.Columns().AdjustToContents();

        // Save to memory stream
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"invoices_report_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private string GenerateInvoiceCsv(IEnumerable<InvoiceReportDto> invoices)
    {
        var sb = new StringBuilder();
        sb.AppendLine("InvoiceNumber,InvoiceDate,TotalAmount,PharmacyName,PaymentMethod,CreditUsed,CashPaid");

        foreach (var invoice in invoices)
        {
            sb.AppendLine($"\"{invoice.InvoiceNumber}\",{invoice.InvoiceDate:yyyy-MM-dd},{invoice.TotalAmount},\"{invoice.PharmacyName}\",{invoice.PaymentMethod},{invoice.CreditUsed},{invoice.CashPaid}");
        }

        return sb.ToString();
    }
}