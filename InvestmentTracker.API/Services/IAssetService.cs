using InvestmentTracker.API.Models;

namespace InvestmentTracker.API.Services;

public interface IAssetService
{
    Task<List<Asset>> GetAllAsync();
    Task<Asset?> GetByIdAsync(int id);
    Task<Asset> CreateAsync(Asset asset);
    Task<Asset?> UpdateAsync(int id, Asset updated);
    Task<bool> DeleteAsync(int id);
    Task<List<AssetWithStatsDto>> GetAssetsWithStatsAsync();
    Task<List<AssetSummaryDto>> GetAssetSummaryAsync(DateTime date);
}
