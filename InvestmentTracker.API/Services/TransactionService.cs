using InvestmentTracker.API.Data;
using InvestmentTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracker.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _db;

    public TransactionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task BulkUpsertAsync(
        int userId,
        DateTime date,
        List<Transaction> txs)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        var assetIds = txs
            .Select(t => t.AssetId)
            .Distinct()
            .ToList();

        var allowedAssetIds = await _db.Assets
            .AsNoTracking()
            .Where(a => a.UserId == userId && assetIds.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync();

        var existing = await _db.Transactions
            .Where(t =>
                allowedAssetIds.Contains(t.AssetId) &&
                t.UserId == userId &&
                t.Date >= dayStart &&
                t.Date < dayEnd)
            .ToListAsync();

        foreach (var tx in txs)
        {
            if (!allowedAssetIds.Contains(tx.AssetId))
                continue;

            var current = existing.FirstOrDefault(t => t.AssetId == tx.AssetId);

            if (current == null)
            {
                tx.UserId = userId;
                tx.Date = dayStart;

                _db.Transactions.Add(tx);
            }
            else
            {
                current.ValueChange  = tx.ValueChange;
                current.CurrentValue = tx.CurrentValue;
                current.Dividend     = tx.Dividend;
                current.Notes        = tx.Notes;
            }
        }

        await _db.SaveChangesAsync();
    }
}
