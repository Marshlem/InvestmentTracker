using InvestmentTracker.API.Data;
using InvestmentTracker.API.DTOs;
using Microsoft.EntityFrameworkCore;

public class AssetSummaryQuery
{
    private readonly ApplicationDbContext _db;

    public AssetSummaryQuery(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<AssetSummaryDto>> ExecuteAsync(int userId, DateTime date)
    {
        return await _db.Assets
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .Select(a => new AssetSummaryDto
            {
                Id = a.Id,
                Name = a.Name,
                Category = a.Category,
                Status = a.Status,
                ValueCurrency = a.ValueChangeCurrency,
                DividendCurrency = a.DividendCurrency,

                TotalInvested = _db.Transactions
                    .Where(t =>
                        t.UserId == userId &&
                        t.AssetId == a.Id &&
                        t.Date <= date &&
                        t.ValueChange > 0)
                    .Sum(t => (decimal?)t.ValueChange) ?? 0m,

                TotalDividends = _db.Transactions
                    .Where(t =>
                        t.UserId == userId &&
                        t.AssetId == a.Id &&
                        t.Date <= date)
                    .Sum(t => (decimal?)t.Dividend) ?? 0m,

                CurrentValue = _db.Transactions
                    .Where(t =>
                        t.UserId == userId &&
                        t.AssetId == a.Id &&
                        t.Date <= date)
                    .OrderByDescending(t => t.Date)
                    .Select(t => t.CurrentValue)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }
}
