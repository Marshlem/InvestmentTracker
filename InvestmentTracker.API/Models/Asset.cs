namespace InvestmentTracker.API.Models;

public class Asset
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public int CategoryId { get; set; }
    public AssetCategory Category { get; set; } = default!;
    public AssetStatus Status { get; set; }  

    public string ValueChangeCurrency { get; set; } = "EUR";
    public string DividendCurrency { get; set; } = "EUR";

    public List<Transaction> Transactions { get; set; } = new();
    public int UserId { get; set; }
    public User User { get; set; } = default!;
}
