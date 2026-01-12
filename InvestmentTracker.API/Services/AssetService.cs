using InvestmentTracker.API.Data;
using InvestmentTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracker.API.Services;

public class AssetService : IAssetService
{
    private readonly ApplicationDbContext _db;
    private readonly AssetStatsQuery _statsQuery;
    private readonly AssetSummaryQuery _summaryQuery;

    public AssetService(
        ApplicationDbContext db,
        AssetStatsQuery statsQuery,
        AssetSummaryQuery summaryQuery)
    {
        _db = db;
        _statsQuery = statsQuery;
        _summaryQuery = summaryQuery;
    }

    public async Task<List<Asset>> GetAllAsync(int userId)
    {
        return await _db.Assets
            .Where(a => a.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Asset?> GetByIdAsync(int userId, int id)
    {
        return await _db.Assets
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
    }

    public async Task<Asset> CreateAsync(int userId, Asset asset)
    {
        asset.UserId = userId;

        _db.Assets.Add(asset);
        await _db.SaveChangesAsync();

        return asset;
    }

    public async Task<Asset?> UpdateAsync(int userId, int id, Asset updated)
    {
        var existing = await _db.Assets
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (existing == null)
            return null;

        _db.Entry(existing).CurrentValues.SetValues(updated);
        existing.UserId = userId; 

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var asset = await _db.Assets
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (asset == null)
            return false;

        _db.Assets.Remove(asset);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<List<AssetWithStatsDto>> GetAssetsWithStatsAsync(int userId)
        => _statsQuery.ExecuteAsync(userId);

    public Task<List<AssetSummaryDto>> GetAssetSummaryAsync(int userId, DateTime date)
        => _summaryQuery.ExecuteAsync(userId, date);
}
