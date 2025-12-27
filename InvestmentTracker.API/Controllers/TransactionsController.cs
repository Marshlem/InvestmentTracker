using Microsoft.AspNetCore.Mvc;
using InvestmentTracker.API.DTOs.Transactions;
using InvestmentTracker.API.Application.Transactions.Commands;
using MediatR;

namespace InvestmentTracker.API.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly TransactionsQuery _query;
    private readonly TransactionsHistoryQuery _historyQuery;


    public TransactionsController(IMediator mediator, TransactionsQuery query, TransactionsHistoryQuery historyQuery)
    {
        _mediator = mediator;
        _query = query;
        _historyQuery = historyQuery;
    }

    // EDIT ekranui
    [HttpGet("edit")]
    public async Task<IActionResult> GetForEdit([FromQuery] DateTime date)
        => Ok(await _query.ExecuteAsync(date));

    // SAVE
    [HttpPost("bulk-upsert")]
    public async Task<IActionResult> BulkUpsert([FromBody] BulkUpsertRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
            return BadRequest("No transactions provided.");

        await _mediator.Send(
            new BulkUpsertTransactionsCommand(request.Date, request.Items)
        );

        return Ok();
    }

    [HttpPost("history")]
    public async Task<IActionResult> History(
        [FromBody] TransactionHistoryRequest request,
        [FromServices] TransactionsHistoryQuery query)
    {
        return Ok(await query.ExecuteAsync(request));
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup()
    {
        return Ok(await _historyQuery.ExecuteAsync());
    }
}