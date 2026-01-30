namespace InvestmentTracker.API.DTOs.TransactionsImport;

public sealed class TransactionImportRow
{
    public int RowNumber { get; set; }

    public DateTime? Date { get; set; }

    public string Asset { get; set; } = "";
    public decimal? ValueChange { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? Dividend { get; set; }
    public string? Notes { get; set; }

    public int? AssetId { get; set; }
    public AssetStatus? AssetStatus { get; set; }

    public bool IsEmpty =>
        ValueChange is null &&
        CurrentValue is null &&
        Dividend is null &&
        string.IsNullOrWhiteSpace(Notes);
}
