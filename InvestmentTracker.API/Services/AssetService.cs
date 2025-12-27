using InvestmentTracker.API.Data;
using InvestmentTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracker.API.Services;

public class AssetService : IAssetService
{
    private readonly ApplicationDbContext _db;
    private readonly AssetStatsQuery _query;
    private readonly AssetSummaryQuery _summaryQuery;
    public AssetService(ApplicationDbContext db, AssetStatsQuery query, AssetSummaryQuery summaryQuery)
    {
        _db = db;
        _query = query;
        _summaryQuery = summaryQuery;
    }


    public async Task<List<Asset>> GetAllAsync()
    {
        return await _db.Assets
            .Include(a => a.Transactions)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Asset?> GetByIdAsync(int id) =>
        await _db.Assets.Include(a => a.Transactions).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Asset> CreateAsync(Asset asset)
    {
        _db.Assets.Add(asset);
        await _db.SaveChangesAsync();
        return asset;
    }

    public async Task<Asset?> UpdateAsync(int id, Asset updated)
    {
        var existing = await _db.Assets.FindAsync(id);
        if (existing == null) return null;

        _db.Entry(existing).CurrentValues.SetValues(updated);
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Assets.FindAsync(id);
        if (entity == null) return false;

        _db.Assets.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<List<AssetWithStatsDto>> GetAssetsWithStatsAsync()
        => _query.ExecuteAsync();

    public Task<List<AssetSummaryDto>> GetAssetSummaryAsync(DateTime date)
        => _summaryQuery.ExecuteAsync(date);
}
