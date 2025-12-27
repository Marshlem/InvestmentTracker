using InvestmentTracker.API.Models;

public record AssetSummaryDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required AssetCategory Category { get; set; } = default!;
    public required AssetStatus Status { get; set; }  

    public required string ValueCurrency { get; init; }
    public required string DividendCurrency { get; init; }

    public decimal TotalInvested { get; init; }
    public decimal CurrentValue { get; init; }
    public decimal TotalDividends { get; init; }
}
