public sealed class DashboardInvestmentTableResponse
{
    public List<DashboardInvestmentTableRow> Rows { get; set; } = [];
    public DashboardInvestmentTableTotal Total { get; set; } = new();
}

public sealed class DashboardInvestmentTableRow
{
    public string Investment { get; set; } = default!;

    public decimal TotalInvested { get; set; }
    public decimal TotalValue { get; set; }
    public decimal TotalDividends { get; set; }

    public decimal GainLoss { get; set; }
    public decimal GainLossPercent { get; set; }

    public decimal GainLossLastMonth { get; set; }
    public decimal GainLossLastMonthPercent { get; set; }

    public decimal InvestedPortfolioPercent { get; set; }
}

public sealed class DashboardInvestmentTableTotal
{
    public decimal TotalInvested { get; set; }
    public decimal TotalValue { get; set; }
    public decimal TotalDividends { get; set; }

    public decimal GainLoss { get; set; }
    public decimal GainLossPercent { get; set; }

    public decimal GainLossLastMonth { get; set; }
    public decimal GainLossLastMonthPercent { get; set; }

    public decimal InvestedPortfolioPercent { get; set; } 
}