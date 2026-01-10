public sealed class DashboardChartResponse
{
    public List<PortfolioValueChartRow> PortfolioValue { get; set; } = [];
    public List<ProfitLossChartRow> ProfitLoss { get; set; } = [];
}

public sealed class PortfolioValueChartRow
{
    public string Period { get; set; } = default!;
    public decimal InvestedAccumulated { get; set; }
    public decimal CurrentValue { get; set; }
}

public sealed class ProfitLossChartRow
{
    public string Period { get; set; } = default!;
    public decimal PeriodProfitLoss { get; set; }
    public decimal AccumulatedProfitLoss { get; set; }
}