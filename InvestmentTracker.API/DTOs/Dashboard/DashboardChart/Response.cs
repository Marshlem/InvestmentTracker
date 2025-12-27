public sealed class DashboardChartResponse
{
    public required List<DashboardChartRowDto> Series { get; set; }
}

public sealed class DashboardChartRowDto
{
    public required string Period { get; set; } 
    public decimal Invested { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal ProfitLoss { get; set; }
}