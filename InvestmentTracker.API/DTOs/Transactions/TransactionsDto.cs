public class TransactionEditDto
{
    public int AssetId { get; set; }
    public string AssetName { get; set; } = "";
    public TransactionEditRowDto? Transaction { get; set; }
}

public class TransactionEditRowDto
{
    public decimal ValueChange { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal Dividend { get; set; }
    public string? Notes { get; set; }
}
