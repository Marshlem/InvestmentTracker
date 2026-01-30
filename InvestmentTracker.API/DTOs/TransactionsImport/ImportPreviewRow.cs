namespace InvestmentTracker.API.DTOs.TransactionsImport;
public sealed class ImportPreviewRow
{
    public int Row { get; set; }
    public DateTime? Date { get; set; }
    public string Asset { get; set; } = "";
    public decimal? ValueChange { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? Dividend { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = ""; // Imported / Skipped / Removed / Error
}