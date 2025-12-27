namespace InvestmentTracker.API.DTOs.Transactions;

public class BulkUpsertRequest
{
    public DateTime Date { get; set; }
    public List<BulkUpsertItem> Items { get; set; } = new();
}

public class BulkUpsertItem
{
    public int AssetId { get; set; }
    public decimal ValueChange { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal Dividend { get; set; }
    public string? Notes { get; set; }
}
