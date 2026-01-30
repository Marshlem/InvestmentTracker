using InvestmentTracker.API.DTOs.Imports;   

namespace InvestmentTracker.API.DTOs.TransactionsImport;

public sealed class TransactionImportPreviewResponse
{
    public bool CanImport { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ImportPreviewRow> Rows { get; set; } = new();
    public List<TransactionImportRow> ImportRows { get; set; } = new();
}

