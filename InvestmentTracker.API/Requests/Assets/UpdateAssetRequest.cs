using InvestmentTracker.API.Models;

namespace InvestmentTracker.API.Requests.Assets;

public class UpdateAssetRequest
{
    public string? Name { get; set; }
    public int CategoryId { get; set; }
    public AssetStatus Status { get; set; }  
    public string? ValueChangeCurrency { get; set; }
    public string? DividendCurrency { get; set; }
}
