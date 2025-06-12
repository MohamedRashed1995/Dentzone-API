// BalancesController.cs
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.DTO.Order;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dragza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class BalancesController : ControllerBase
{
    private readonly IBalanceService _balanceService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BalancesController(IBalanceService balanceService, IHttpContextAccessor httpContextAccessor)
    {
        _balanceService = balanceService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserBalances(Guid userId)
    {
        // Verify current user has access to these balances
        //var currentUserId = GetCurrentUserId();
        //if (currentUserId != userId && !User.IsInRole("Admin"))
        //{
        //    return Forbid();
        //}

        var balances = await _balanceService.GetUserBalances(userId);
        return Ok(balances);
    }

    [HttpPost("deposit/cash")]
    //[Authorize()]
    public async Task<IActionResult> DepositToCash([FromBody] UpdateBalanceRequestDto request)
    {
        var balances = await _balanceService.GetUserBalances(request.UserId);

        var transaction = await _balanceService.CreateTransaction(
            balances.CashAccount.Id,
            request.Amount,
            request.UserId,
            TransactionType.Deposit,
            request.Description);

        return Ok(transaction);
    }

    [HttpPost("withdraw/cash")]
    [Authorize(Roles = "Pharmacy")]
    public async Task<IActionResult> WithdrawFromCash([FromBody] UpdateBalanceRequestDto request)
    {
        var balances = await _balanceService.GetUserBalances(request.UserId);

        if (!await _balanceService.HasSufficientBalance(balances.CashAccount.Id, -request.Amount))
        {
            return BadRequest("Insufficient cash balance");
        }

        var transaction = await _balanceService.CreateTransaction(
            balances.CashAccount.Id,
            -request.Amount,
            request.UserId,
            TransactionType.Withdrawal,
            request.Description);

        return Ok(transaction);
    }

    [HttpPut("credit-limit")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCreditLimit([FromBody] UpdateCreditLimitDto request)
    {
        var account = await _balanceService.UpdateCreditLimit(request.AccountId, request.NewLimit);
        return Ok(account);
    }

    [HttpGet("transactions/{accountId}")]
    public async Task<IActionResult> GetAccountTransactions(Guid accountId)
    {
        var transactions = await _balanceService.GetAccountTransactions(accountId);
        return Ok(transactions);
    }

    [HttpGet("user-transactions/{userId}")]
    public async Task<IActionResult> GetUserTransactions(Guid userId)
    {
        var transactions = await _balanceService.GetUserTransactions(userId);
        return Ok(transactions);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity");

        return userId;
    }
}
