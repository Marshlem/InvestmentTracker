using InvestmentTracker.API.Data;
using InvestmentTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracker.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _db;
    public TransactionService(ApplicationDbContext db) => _db = db;

    public async Task BulkUpsertAsync(DateTime date, List<Transaction> txs)
    {
        var nextDay = date.Date.AddDays(1);
        var assetIds = txs.Select(t => t.AssetId).Distinct().ToList();

        var existingByAsset = await _db.Transactions
            .Where(t => assetIds.Contains(t.AssetId)
                        && t.Date >= date.Date
                        && t.Date < nextDay)
            .ToListAsync();

        foreach (var tx in txs)
        {
            var existing = existingByAsset.FirstOrDefault(t => t.AssetId == tx.AssetId);

            if (existing == null)
            {
                tx.Date = date.Date;
                _db.Transactions.Add(tx);
            }
            else
            {
                existing.ValueChange  = tx.ValueChange;
                existing.CurrentValue = tx.CurrentValue;
                existing.Dividend     = tx.Dividend;
                existing.Notes        = tx.Notes;
            }
        }

        await _db.SaveChangesAsync();
    }

}
