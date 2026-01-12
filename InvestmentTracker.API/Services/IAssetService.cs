using InvestmentTracker.API.Models;

namespace InvestmentTracker.API.Services;

public interface IAssetService
{
    Task<List<Asset>> GetAllAsync(int userId);
    Task<Asset?> GetByIdAsync(int userId, int id);
    Task<Asset> CreateAsync(int userId, Asset asset);
    Task<Asset?> UpdateAsync(int userId, int id, Asset updated);
    Task<bool> DeleteAsync(int userId, int id);

    Task<List<AssetWithStatsDto>> GetAssetsWithStatsAsync(int userId);
    Task<List<AssetSummaryDto>> GetAssetSummaryAsync(int userId, DateTime date);
}
