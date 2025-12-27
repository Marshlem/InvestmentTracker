using InvestmentTracker.API.Data;
using InvestmentTracker.API.DTOs;
using Microsoft.EntityFrameworkCore;

public class DashboardSummaryQuery
{
    private readonly ApplicationDbContext _db;

    public DashboardSummaryQuery(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardSummaryDto> ExecuteAsync(DateTime date)
    {
        var tx = _db.Transactions.Where(t => t.Date <= date);

        var invested = await tx
            .Where(t => t.ValueChange > 0)
            .SumAsync(t => (decimal?)t.ValueChange) ?? 0;

        var dividends = await tx
            .SumAsync(t => (decimal?)t.Dividend) ?? 0;

        var currentValue = await tx
            .GroupBy(t => t.AssetId)
            .Select(g => g
                .OrderByDescending(x => x.Date)
                .Select(x => x.CurrentValue)
                .FirstOrDefault())
            .SumAsync(x => (decimal?)x) ?? 0;

        return new DashboardSummaryDto
        {
            TotalInvested = invested,
            CurrentValue = currentValue,
            TotalDividends = dividends,
            ProfitLoss = currentValue - invested + dividends
        };
    }

public async Task<DashboardChartResponse> GetChart(DashboardChartRequest req)
{
    var tx = _db.Transactions
        .AsNoTracking()
        .Where(t =>
            t.Date >= req.DateFrom &&
            t.Date <= req.DateTo);

    if (req.AssetIds != null && req.AssetIds.Count > 0)
        tx = tx.Where(t => req.AssetIds.Contains(t.AssetId));

    // 1. Priskiriam periodą
    var withPeriod = tx.Select(t => new
    {
        t.AssetId,
        t.Date,
        t.ValueChange,
        t.Dividend,
        t.CurrentValue,
        Period =
            req.GroupBy == "month"
                ? $"{t.Date.Year}-{t.Date.Month:00}"
            : req.GroupBy == "quarter"
                ? $"{t.Date.Year}-Q{((t.Date.Month - 1) / 3 + 1)}"
            : t.Date.Year.ToString()
    });

    // 2. Grupė: Period + Asset
var perAsset = await tx
    .GroupBy(t => new
    {
        t.AssetId,
        Year = t.Date.Year,
        PeriodIndex =
            req.GroupBy == "month"
                ? t.Date.Month
                : req.GroupBy == "quarter"
                    ? ((t.Date.Month - 1) / 3 + 1)
                    : 1
    })
    .Select(g => new
    {
        g.Key.Year,
        g.Key.PeriodIndex,
        g.Key.AssetId,

        Invested = g.Where(x => x.ValueChange > 0)
                    .Sum(x => (decimal?)x.ValueChange) ?? 0,

        Dividends = g.Sum(x => (decimal?)x.Dividend) ?? 0,

        CurrentValue = g
            .OrderByDescending(x => x.Date)
            .Select(x => x.CurrentValue)
            .FirstOrDefault()
    })
    .ToListAsync();

    // 3. Grupė tik pagal Period
var series = perAsset
    .GroupBy(x => new { x.Year, x.PeriodIndex })
    .OrderBy(g => g.Key.Year)
    .ThenBy(g => g.Key.PeriodIndex)
    .Select(g => new DashboardChartRowDto
    {
        Period = req.GroupBy switch
        {
            "month"   => $"{g.Key.Year}-{g.Key.PeriodIndex:D2}",
            "quarter" => $"{g.Key.Year}-Q{g.Key.PeriodIndex}",
            _         => g.Key.Year.ToString()
        },
        Invested = g.Sum(x => x.Invested),
        CurrentValue = g.Sum(x => x.CurrentValue),
        ProfitLoss = g.Sum(x => x.CurrentValue - x.Invested + x.Dividends)
    })
    .ToList();


    return new DashboardChartResponse
    {
        Series = series
    };
}

}
