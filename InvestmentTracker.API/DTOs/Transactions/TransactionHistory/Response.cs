public class TransactionHistoryResponse
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    public int AssetId { get; set; }
    public string AssetName { get; set; } = "";

    public decimal ValueChange { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal Dividend { get; set; }
    public string? Notes { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
}
