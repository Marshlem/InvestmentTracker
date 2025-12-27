using InvestmentTracker.API.Data;
using InvestmentTracker.API.DTOs;
using Microsoft.EntityFrameworkCore;


public class TransactionsHistoryQuery
{
    private readonly ApplicationDbContext _db;
    public TransactionsHistoryQuery(ApplicationDbContext db) => _db = db;

    public async Task<PagedResult<TransactionHistoryResponse>> ExecuteAsync(TransactionHistoryRequest req)
    {
        var q = _db.Transactions
            .AsNoTracking()
            .Where(t => t.Date >= req.DateFrom && t.Date <= req.DateTo);

        if (req.AssetId.HasValue)
            q = q.Where(t => t.AssetId == req.AssetId.Value);

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(t => new TransactionHistoryResponse
            {
                Id = t.Id,
                Date = t.Date,
                AssetId = t.AssetId,
                AssetName = t.Asset!.Name,
                ValueChange = t.ValueChange,
                CurrentValue = t.CurrentValue,
                Dividend = t.Dividend,
                Notes = t.Notes
            })
            .ToListAsync();

        return new PagedResult<TransactionHistoryResponse>
        {
            Items = items,
            Total = total
        };
    }

        public async Task<List<TransactionsDropList>> ExecuteAsync()
    {
        return await _db.Assets
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .Select(a => new TransactionsDropList
            {
                Id = a.Id,
                Name = a.Name
            })
            .ToListAsync();
    }
}
