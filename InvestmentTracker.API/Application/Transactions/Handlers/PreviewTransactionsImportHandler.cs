using InvestmentTracker.API.DTOs.TransactionsImport;
using InvestmentTracker.API.Infrastructure.Imports.Excel;
using InvestmentTracker.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InvestmentTracker.API.Validators;

namespace InvestmentTracker.API.Application.Transactions.Imports;

public sealed record PreviewTransactionsImportQuery(
    int UserId,
    Stream FileStream
) : IRequest<TransactionImportPreviewResponse>;

public sealed class PreviewTransactionsImportQueryHandler
    : IRequestHandler<PreviewTransactionsImportQuery, TransactionImportPreviewResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly ExcelTransactionImportReader _reader;

    public PreviewTransactionsImportQueryHandler(
        ApplicationDbContext db,
        ExcelTransactionImportReader reader)
    {
        _db = db;
        _reader = reader;
    }

    public async Task<TransactionImportPreviewResponse> Handle(
        PreviewTransactionsImportQuery request,
        CancellationToken cancellationToken)
    {
        var rows = _reader.Read(request.FileStream);

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

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Asset))
                continue;

            if (!assetDict.TryGetValue(row.Asset.Trim(), out var asset))
                continue;

            row.AssetId = asset.Id;
            row.AssetStatus = asset.Status;
        }

        var validation = TransactionImportValidator.Validate(rows);

        var previewRows = rows.Select(r => new ImportPreviewRow
        {
            Row = r.RowNumber,
            Date = r.Date,
            Asset = r.Asset,
            ValueChange = r.ValueChange,
            CurrentValue = r.CurrentValue,
            Dividend = r.Dividend,
            Notes = r.Notes,
            Status = ResolveStatus(r)
        }).ToList();

        return new TransactionImportPreviewResponse
        {
            CanImport = validation.IsValid,
            Errors = validation.Errors,
            Rows = previewRows,
            ImportRows = rows
        };
    }

    private static string ResolveStatus(TransactionImportRow row)
    {
        if (row.AssetId is null)
            return "Error (asset not found)";

        if (row.IsEmpty && row.AssetStatus == AssetStatus.Inactive)
            return "Ignored (removed asset)";

        if (row.IsEmpty && row.AssetStatus == AssetStatus.Active)
            return "Skipped (keeping previous values)";

        return "Will be imported";
    }
}
