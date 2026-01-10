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

    public async Task<DashboardInvestmentTableResponse> GetInvestmentTable()
    {
        var tx = _db.Transactions
            .AsNoTracking()
            .AsQueryable();

        // -------------------------
        // Datos ribos
        // -------------------------
        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);
        var prevMonthEnd = thisMonthStart.AddDays(-1);
        var prevMonthStart = new DateTime(prevMonthEnd.Year, prevMonthEnd.Month, 1);

        // -------------------------
        // Praeito mėnesio paskutinė CurrentValue (per asset)
        // -------------------------
        var prevMonthValues = await _db.Transactions
            .AsNoTracking()
            .Where(t => t.Date >= prevMonthStart && t.Date <= prevMonthEnd)
            .GroupBy(t => t.AssetId)
            .Select(g => new
            {
                AssetId = g.Key,
                Value = g
                    .OrderByDescending(x => x.Date)
                    .Select(x => x.CurrentValue)
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.AssetId, x => x.Value);

        // -------------------------
        // Einamo mėnesio dividendai (per asset)
        // -------------------------
        var thisMonthDividends = await _db.Transactions
            .AsNoTracking()
            .Where(t => t.Date >= thisMonthStart)
            .GroupBy(t => t.AssetId)
            .Select(g => new
            {
                AssetId = g.Key,
                Dividends = g.Sum(x => (decimal?)x.Dividend) ?? 0
            })
            .ToDictionaryAsync(x => x.AssetId, x => x.Dividends);

        // -------------------------
        // Pagrindinė agregacija per asset
        // -------------------------
        var perAsset = await (
            from t in tx
            join a in _db.Assets.AsNoTracking()
                on t.AssetId equals a.Id
            group t by new { t.AssetId, a.Name } into g
            select new
            {
                AssetId = g.Key.AssetId,
                Name = g.Key.Name,

                TotalInvested = g
                    .Where(x => x.ValueChange > 0)
                    .Sum(x => (decimal?)x.ValueChange) ?? 0,

                TotalWithdrawn = g
                    .Where(x => x.ValueChange < 0)
                    .Sum(x => (decimal?)x.ValueChange) ?? 0,

                TotalDividends = g.Sum(x => (decimal?)x.Dividend) ?? 0,

                LatestValue = g
                    .OrderByDescending(x => x.Date)
                    .Select(x => x.CurrentValue)
                    .FirstOrDefault()
            }
        ).ToListAsync();

        var portfolioTotalInvested = perAsset.Sum(x => x.TotalInvested);

        // -------------------------
        // Formuojam eilutes
        // -------------------------
        var rows = perAsset.Select(x =>
        {
            var gainLoss =
                x.LatestValue
                - x.TotalInvested
                + x.TotalWithdrawn
                + x.TotalDividends;

            var prevValue = prevMonthValues.TryGetValue(x.AssetId, out var pv)
                ? pv
                : 0;

            var dividendsThisMonth = thisMonthDividends.TryGetValue(x.AssetId, out var d)
                ? d
                : 0;

            var gainLossLastMonth =
                prevValue == 0
                    ? 0
                    : (x.LatestValue + dividendsThisMonth) - prevValue;

            var gainLossLastMonthPercent =
                prevValue == 0
                    ? 0
                    : gainLossLastMonth / prevValue * 100;

            return new DashboardInvestmentTableRow
            {
                Investment = x.Name,
                TotalInvested = x.TotalInvested,
                TotalValue = x.LatestValue,
                TotalDividends = x.TotalDividends,

                GainLoss = gainLoss,
                GainLossPercent = x.TotalInvested == 0
                    ? 0
                    : gainLoss / x.TotalInvested * 100,

                GainLossLastMonth = gainLossLastMonth,
                GainLossLastMonthPercent = gainLossLastMonthPercent,

                InvestedPortfolioPercent = portfolioTotalInvested == 0
                    ? 0
                    : x.TotalInvested / portfolioTotalInvested * 100
            };
        }).ToList();

        // -------------------------
        // TOTAL
        // -------------------------
        var totalInvested = rows.Sum(x => x.TotalInvested);
        var totalGainLoss = rows.Sum(x => x.GainLoss);
        var totalGainLossLastMonth = rows.Sum(x => x.GainLossLastMonth);
        var portfolioValuePrevMonth = prevMonthValues.Values.Sum();

        var total = new DashboardInvestmentTableTotal
        {
            TotalInvested = totalInvested,
            TotalValue = rows.Sum(x => x.TotalValue),
            TotalDividends = rows.Sum(x => x.TotalDividends),

            GainLoss = totalGainLoss,

            GainLossPercent = totalInvested == 0
                ? 0
                : totalGainLoss / totalInvested * 100,

            GainLossLastMonth = totalGainLossLastMonth,

            GainLossLastMonthPercent =
                portfolioValuePrevMonth == 0
                    ? 0
                    : totalGainLossLastMonth / portfolioValuePrevMonth * 100,

            InvestedPortfolioPercent = 100
        };

        return new DashboardInvestmentTableResponse
        {
            Rows = rows,
            Total = total
        };
    }
}
