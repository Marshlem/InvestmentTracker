using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InvestmentTracker.API.DTOs.Transactions;
using InvestmentTracker.API.Application.Transactions.Commands;
using InvestmentTracker.API.Infrastructure;
using MediatR;

namespace InvestmentTracker.API.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public sealed class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly TransactionsQuery _query;
    private readonly TransactionsHistoryQuery _historyQuery;

    public TransactionsController(
        IMediator mediator,
        TransactionsQuery query,
        TransactionsHistoryQuery historyQuery)
    {
        _mediator = mediator;
        _query = query;
        _historyQuery = historyQuery;
    }

    // =========================
    // EDIT screen (daily input)
    // =========================
    [HttpGet("edit")]
    public async Task<IActionResult> GetForEdit([FromQuery] DateTime date)
    {
        var userId = UserContext.GetUserId(User);
        var result = await _query.ExecuteAsync(userId, date);
        return Ok(result);
    }

    // =========================
    // SAVE (bulk upsert)
    // =========================
    [HttpPost("bulk-upsert")]
    public async Task<IActionResult> BulkUpsert([FromBody] BulkUpsertRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
            return BadRequest("No transactions provided.");

        var userId = UserContext.GetUserId(User);

        await _mediator.Send(
            new BulkUpsertTransactionsCommand(
                userId,
                request.Date,
                request.Items
            )
        );

        return Ok();
    }

    // =========================
    // HISTORY (paged)
    // =========================
    [HttpPost("history")]
    public async Task<IActionResult> History([FromBody] TransactionHistoryRequest request)
    {
        var userId = UserContext.GetUserId(User);
        var result = await _historyQuery.ExecuteAsync(userId, request);
        return Ok(result);
    }

    // =========================
    // LOOKUP (dropdown)
    // =========================
    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup()
    {
        var userId = UserContext.GetUserId(User);
        var result = await _historyQuery.GetLookupAsync(userId);
        return Ok(result);
    }
}
