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
        var groupBy = string.IsNullOrWhiteSpace(req.GroupBy)
            ? "month"
            : req.GroupBy;

        var tx = _db.Transactions
            .AsNoTracking()
            .AsQueryable();

        if (req.DateFrom.HasValue)
            tx = tx.Where(t => t.Date >= req.DateFrom.Value);

        if (req.DateTo.HasValue)
            tx = tx.Where(t => t.Date <= req.DateTo.Value);

        if (req.AssetIds is { Count: > 0 })
            tx = tx.Where(t => req.AssetIds.Contains(t.AssetId));

        // 1. Grupavimas: Period + Asset (kaip ir buvo)
        var perAsset = await tx
            .GroupBy(t => new
            {
                t.AssetId,
                Year = t.Date.Year,
                PeriodIndex =
                    groupBy == "month"
                        ? t.Date.Month
                        : groupBy == "quarter"
                            ? ((t.Date.Month - 1) / 3 + 1)
                            : 1
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.PeriodIndex,
                g.Key.AssetId,

                Invested = g
                    .Where(x => x.ValueChange > 0)
                    .Sum(x => (decimal?)x.ValueChange) ?? 0,

                Dividends = g.Sum(x => (decimal?)x.Dividend) ?? 0,

                CurrentValue = g
                    .OrderByDescending(x => x.Date)
                    .Select(x => x.CurrentValue)
                    .FirstOrDefault()
            })
            .ToListAsync();

        // 2. Grupavimas tik pagal periodą (bazinė serija abiem grafikams)
        var perPeriod = perAsset
            .GroupBy(x => new { x.Year, x.PeriodIndex })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.PeriodIndex)
            .Select(g => new
            {
                Period = groupBy switch
                {
                    "month"   => $"{g.Key.Year}-{g.Key.PeriodIndex:D2}",
                    "quarter" => $"{g.Key.Year}-Q{g.Key.PeriodIndex}",
                    _         => g.Key.Year.ToString()
                },

                InvestedPeriod = g.Sum(x => x.Invested),
                DividendsPeriod = g.Sum(x => x.Dividends),
                CurrentValue = g.Sum(x => x.CurrentValue)
            })
            .ToList();

        // 3. Grafikas #1 – Portfolio Value
        decimal investedAcc = 0;
        var portfolioValue = new List<PortfolioValueChartRow>(perPeriod.Count);

        foreach (var p in perPeriod)
        {
            investedAcc += p.InvestedPeriod;

            portfolioValue.Add(new PortfolioValueChartRow
            {
                Period = p.Period,
                InvestedAccumulated = investedAcc,
                CurrentValue = p.CurrentValue
            });
        }

        // 4. Grafikas #2 – Profit / Loss
        decimal profitLossAcc = 0;
        decimal prevValue = 0;

        var profitLoss = new List<ProfitLossChartRow>(perPeriod.Count);

        foreach (var p in perPeriod)
        {
            var periodProfitLoss =
                p.CurrentValue
                - prevValue
                - p.InvestedPeriod
                + p.DividendsPeriod;

            profitLossAcc += periodProfitLoss;

            profitLoss.Add(new ProfitLossChartRow
            {
                Period = p.Period,
                PeriodProfitLoss = periodProfitLoss,
                AccumulatedProfitLoss = profitLossAcc
            });

            prevValue = p.CurrentValue;
        }

        return new DashboardChartResponse
        {
            PortfolioValue = portfolioValue,
            ProfitLoss = profitLoss
        };
    }
}
