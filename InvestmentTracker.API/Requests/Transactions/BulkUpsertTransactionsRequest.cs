namespace InvestmentTracker.API.Requests.Transactions;

public class BulkUpsertTransactionsRequest
{
    public DateTime Date { get; set; }
    public List<CreateTransactionRequest> Items { get; set; } = new();
}
