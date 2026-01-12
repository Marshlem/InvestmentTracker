namespace InvestmentTracker.API.Models;

public class Transaction
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public DateTime Date { get; set; }
    public decimal ValueChange { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal Dividend { get; set; }
    public string? Notes { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = default!;
}
