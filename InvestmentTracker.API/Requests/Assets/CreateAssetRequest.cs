using InvestmentTracker.API.Models;

namespace InvestmentTracker.API.Requests.Assets;

public class CreateAssetRequest
{
    public string Name { get; set; } = default!;
    public int CategoryId { get; set; }
    public AssetStatus Status { get; set; }
    public string ValueChangeCurrency { get; set; } = "EUR";
    public string DividendCurrency { get; set; } = "EUR";
}
