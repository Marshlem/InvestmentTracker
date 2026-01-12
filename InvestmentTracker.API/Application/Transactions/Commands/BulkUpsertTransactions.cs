using MediatR;
using InvestmentTracker.API.DTOs.Transactions;

namespace InvestmentTracker.API.Application.Transactions.Commands;

public sealed class BulkUpsertTransactionsCommand
    : IRequest<BulkUpsertResult>
{
    public int UserId { get; }
    public DateTime Date { get; }
    public List<BulkUpsertItem> Items { get; }

    public BulkUpsertTransactionsCommand(
        int userId,
        DateTime date,
        List<BulkUpsertItem> items)
    {
        UserId = userId;
        Date = date;
        Items = items;
    }
}

public sealed class BulkUpsertResult
{
    public bool Success { get; init; }
    public int Count { get; init; }
}
