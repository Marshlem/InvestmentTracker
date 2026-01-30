using InvestmentTracker.API.DTOs.Imports;
using InvestmentTracker.API.DTOs.TransactionsImport;
using InvestmentTracker.API.Data;
using InvestmentTracker.API.DTOs.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InvestmentTracker.API.Application.Transactions.Commands;
using InvestmentTracker.API.Validators;

namespace InvestmentTracker.API.Application.Transactions.Imports;

// ❌ Date nebereikalinga – datos ateina iš eilučių
public sealed record ConfirmTransactionsImportCommand(
    int UserId,
    List<TransactionImportRow> Rows
) : IRequest<ValidationResult<ConfirmTransactionsImportResponse>>;

public sealed class ConfirmTransactionsImportResponse
{
    public int ImportedCount { get; set; }
    public int SkippedActiveEmptyCount { get; set; }
    public int IgnoredRemovedEmptyCount { get; set; }
}

public sealed class ConfirmTransactionsImportCommandHandler
    : IRequestHandler<ConfirmTransactionsImportCommand, ValidationResult<ConfirmTransactionsImportResponse>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMediator _mediator;

    public ConfirmTransactionsImportCommandHandler(
        ApplicationDbContext db,
        IMediator mediator)
    {
        _db = db;
        _mediator = mediator;
    }

    public async Task<ValidationResult<ConfirmTransactionsImportResponse>> Handle(
        ConfirmTransactionsImportCommand request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ Resolve assets (by user)
        var assets = await _db.Assets
            .Where(a => a.UserId == request.UserId)
            .Select(a => new
            {
                a.Id,
                a.Name,
                a.Status
            })
            .ToListAsync(cancellationToken);

        var assetDict = assets.ToDictionary(
            a => a.Name.Trim(),
            a => a,
            StringComparer.OrdinalIgnoreCase);

        // 2️⃣ Attach AssetId + AssetStatus to rows
        foreach (var row in request.Rows)
        {
            if (string.IsNullOrWhiteSpace(row.Asset))
                continue;

            if (!assetDict.TryGetValue(row.Asset.Trim(), out var asset))
                continue;

            row.AssetId = asset.Id;
            row.AssetStatus = asset.Status;
        }

        // 3️⃣ Validate rows (DATE PER ROW)
        var validation = TransactionImportValidator.Validate(request.Rows);

        if (!validation.IsValid || validation.Value is null)
        {
            var res = new ValidationResult<ConfirmTransactionsImportResponse>();
            res.Errors.AddRange(validation.Errors);
            return res;
        }

        // 4️⃣ Stats for UI
        var skippedActiveEmpty = request.Rows.Count(r =>
            r.IsEmpty && r.AssetStatus == AssetStatus.Active);

        var ignoredRemovedEmpty = request.Rows.Count(r =>
            r.IsEmpty && r.AssetStatus == AssetStatus.Inactive);

        // 5️⃣ Group by Date and execute bulk upsert per date
        var groupedRows = request.Rows
            .Where(r =>
                !r.IsEmpty &&
                r.AssetId.HasValue &&
                r.Date.HasValue)
            .GroupBy(r => r.Date!.Value.Date);

            foreach (var group in groupedRows)
            {
                var items = group.Select(r => new BulkUpsertItem
                {
                    AssetId = r.AssetId!.Value,
                    ValueChange = r.ValueChange!.Value,
                    CurrentValue = r.CurrentValue!.Value,
                    Dividend = r.Dividend!.Value,
                    Notes = r.Notes
                }).ToList();

                await _mediator.Send(
                    new BulkUpsertTransactionsCommand(
                        request.UserId,
                        group.Key, // <-- Date
                        items
                    ),
                    cancellationToken);
            }

        return new ValidationResult<ConfirmTransactionsImportResponse>
        {
            Value = new ConfirmTransactionsImportResponse
            {
                ImportedCount = validation.Value.Count,
                SkippedActiveEmptyCount = skippedActiveEmpty,
                IgnoredRemovedEmptyCount = ignoredRemovedEmpty
            }
        };
    }
}
