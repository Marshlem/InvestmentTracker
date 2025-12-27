namespace InvestmentTracker.API.Requests.Transactions;

public class CreateTransactionRequest
{
    public int AssetId { get; set; }
    public DateTime Date { get; set; }
    public decimal ValueChange { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal Dividend { get; set; }
    public string? Notes { get; set; }
}
