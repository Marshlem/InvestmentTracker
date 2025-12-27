using InvestmentTracker.API.Models;

public sealed record AssetWithStatsDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public AssetCategory? Category { get; set; }
    public AssetStatus? Status { get; set; }  

    public required string ValueCurrency { get; init; }
    public required string DividendCurrency { get; init; }

    public decimal TotalInvested { get; init; }
    public decimal CurrentValue { get; init; }
    public decimal TotalDividends { get; init; }
}
