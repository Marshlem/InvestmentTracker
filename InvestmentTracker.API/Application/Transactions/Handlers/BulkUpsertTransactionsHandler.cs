using MediatR;
using InvestmentTracker.API.Application.Transactions.Commands;
using InvestmentTracker.API.Models;
using InvestmentTracker.API.Services;

namespace InvestmentTracker.API.Application.Transactions.Handlers;

public sealed class BulkUpsertTransactionsHandler : IRequestHandler<BulkUpsertTransactionsCommand, BulkUpsertResult>
{
    private readonly ITransactionService _txService;

    public BulkUpsertTransactionsHandler(ITransactionService txService)
        => _txService = txService;

    public async Task<BulkUpsertResult> Handle(BulkUpsertTransactionsCommand request, CancellationToken ct)
    {
        if (request.Items is null || request.Items.Count == 0)
            return new BulkUpsertResult { Success = false, Count = 0 };

        var entities = request.Items.Select(i => new Transaction
        {
            AssetId      = i.AssetId,
            Date         = request.Date,
            ValueChange  = i.ValueChange,
            CurrentValue = i.CurrentValue,
            Dividend     = i.Dividend,
            Notes        = i.Notes
        }).ToList();

        await _txService.BulkUpsertAsync(request.Date, entities);

        return new BulkUpsertResult { Success = true, Count = entities.Count };
    }
}
