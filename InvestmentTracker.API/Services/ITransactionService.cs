using InvestmentTracker.API.Models;

namespace InvestmentTracker.API.Services;

public interface ITransactionService
{
    Task BulkUpsertAsync(DateTime date, List<Transaction> txs);
}
