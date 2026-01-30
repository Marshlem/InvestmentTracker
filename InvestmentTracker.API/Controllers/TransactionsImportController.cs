using InvestmentTracker.API.Application.Transactions.Imports;
using InvestmentTracker.API.DTOs.TransactionsImport;
using InvestmentTracker.API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracker.API.Controllers;

[ApiController]
[Route("api/transactions/import")]
[Authorize]
public sealed class TransactionsImportController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsImportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =========================
    // PREVIEW EXCEL IMPORT
    // =========================
    [HttpPost("preview")]
    [RequestSizeLimit(10_000_000)] // 10MB
    public async Task<IActionResult> Preview(
        [FromForm] IFormFile file,
        [FromForm] DateTime date)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Only .xlsx supported");

        var userId = UserContext.GetUserId(User);

        using var stream = file.OpenReadStream();

        var result = await _mediator.Send(
            new PreviewTransactionsImportQuery(
                userId,
                stream));

        return Ok(result);
    }

    // =========================
    // CONFIRM IMPORT
    // =========================
    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] TransactionImportConfirmRequest request)
    {
        if (request.Rows == null || request.Rows.Count == 0)
            return BadRequest("No rows to import");

        var userId = UserContext.GetUserId(User);

        var result = await _mediator.Send(
            new ConfirmTransactionsImportCommand(
                userId,
                request.Rows));

        if (!result.IsValid)
            return BadRequest(result);

        return Ok(result.Value);
    }
}
