// BalanceReportsController.cs
using ClosedXML.Excel;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Dragza.Api.Controllers;

[ApiController]
[Route("api/reports/balance")]
[Authorize(Roles = "Admin,Finance")]
public class BalanceReportsController : ControllerBase
{
    private readonly IBalanceReportingService _reportingService;

    public BalanceReportsController(IBalanceReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet("accounts")]
    public async Task<ActionResult<PaginatedResult<BalanceAccountReportDto>>> GetBalanceAccountsReport(
        [FromQuery] BalanceReportFilterDto filter)
    {
        var result = await _reportingService.GetBalanceAccountsReportAsync(filter);
        return Ok(result);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<PaginatedResult<BalanceTransactionReportDto>>> GetBalanceTransactionsReport(
        [FromQuery] BalanceReportFilterDto filter)
    {
        var result = await _reportingService.GetBalanceTransactionsReportAsync(filter);
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<BalanceSummaryDto>> GetBalanceSummary(
        [FromQuery] BalanceReportFilterDto filter)
    {
        var result = await _reportingService.GetBalanceSummaryAsync(filter);
        return Ok(result);
    }

    [HttpGet("export/transactions")]
    public async Task<IActionResult> ExportTransactionsReport(
        [FromQuery] BalanceReportFilterDto filter)
    {
        filter.PageNumber = 1;
        filter.PageSize = int.MaxValue;

        var result = await _reportingService.GetBalanceTransactionsReportAsync(filter);
        // Create a new Excel workbook
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Transactions");

        // Add headers
        var headers = new string[] { "TransactionDate", "Amount", "Type", "Description", "User", "OrderNumber" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        // Add data rows
        int currentRow = 2;
        foreach (var t in result.Items)
        {
            worksheet.Cell(currentRow, 1).Value = t.TransactionDate;
            worksheet.Cell(currentRow, 1).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
            worksheet.Cell(currentRow, 2).Value = t.Amount;
            worksheet.Cell(currentRow, 3).Value = t.TransactionType;
            worksheet.Cell(currentRow, 4).Value = t.Description;
            worksheet.Cell(currentRow, 5).Value = t.UserName;
            worksheet.Cell(currentRow, 6).Value = t.OrderId.HasValue ? t.OrderId.ToString() : "N/A";
            currentRow++;
        }

        // Format the header row
        var headerRange = worksheet.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Format amount column (assuming it's column B)
        worksheet.Column(2).Style.NumberFormat.Format = "#,##0.00";

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Save to memory stream
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"balance_transactions_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        //var csv = GenerateTransactionsCsv(result.Items);

        //return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"balance_transactions_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    private string GenerateTransactionsCsv(IEnumerable<BalanceTransactionReportDto> transactions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("TransactionDate,Amount,Type,Description,User,OrderNumber");

        foreach (var t in transactions)
        {
            sb.AppendLine($"\"{t.TransactionDate:yyyy-MM-dd HH:mm}\",{t.Amount},\"{t.TransactionType}\",\"{t.Description}\",\"{t.UserName}\",{(t.OrderId.HasValue ? t.OrderId.ToString() : "N/A")}");
        }

        return sb.ToString();
    }
}