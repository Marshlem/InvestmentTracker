using InvestmentTracker.API.Data;
using InvestmentTracker.API.DTOs;
using Microsoft.EntityFrameworkCore;

public class TransactionsQuery
{
    private readonly ApplicationDbContext _db;

    public TransactionsQuery(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<TransactionEditDto>> ExecuteAsync(int userId, DateTime date)
    {
        var nextDay = date.Date.AddDays(1);

        return await _db.Assets
            .AsNoTracking()
            .Where(a =>
                a.UserId == userId &&
                a.Status == AssetStatus.Active)
            .Select(a => new
            {
                Asset = a,
                Tx = _db.Transactions
                    .Where(t =>
                        t.UserId == userId &&
                        t.AssetId == a.Id &&
                        t.Date >= date.Date &&
                        t.Date < nextDay)
                    .FirstOrDefault()
            })
            .Select(x => new TransactionEditDto
            {
                AssetId = x.Asset.Id,
                AssetName = x.Asset.Name,
                Transaction = x.Tx == null
                    ? null
                    : new TransactionEditRowDto
                    {
                        ValueChange = x.Tx.ValueChange,
                        CurrentValue = x.Tx.CurrentValue,
                        Dividend = x.Tx.Dividend,
                        Notes = x.Tx.Notes
                    }
            })
            .ToListAsync();
    }
}
