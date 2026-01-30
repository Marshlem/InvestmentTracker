namespace InvestmentTracker.API.DTOs.TransactionsImport;

public sealed class TransactionImportConfirmRequest
{
    public List<TransactionImportRow> Rows { get; set; } = new();
}