using InvestmentTracker.API.Models;

namespace InvestmentTracker.API.Services;

public interface ITransactionService
{
    Task BulkUpsertAsync(int userId, DateTime date, List<Transaction> txs);
}
