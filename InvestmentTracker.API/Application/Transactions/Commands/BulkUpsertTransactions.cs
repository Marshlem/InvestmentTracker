using MediatR;
using InvestmentTracker.API.DTOs.Transactions;

namespace InvestmentTracker.API.Application.Transactions.Commands;

public record BulkUpsertTransactionsCommand(DateTime Date, List<BulkUpsertItem> Items)
    : IRequest<BulkUpsertResult>;

public sealed class BulkUpsertResult
{
    public bool Success { get; init; }
    public int Count { get; init; }
}
