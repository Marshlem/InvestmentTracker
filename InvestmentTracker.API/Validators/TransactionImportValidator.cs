using InvestmentTracker.API.DTOs.Imports;
using InvestmentTracker.API.DTOs.Transactions;
using InvestmentTracker.API.DTOs.TransactionsImport;
using InvestmentTracker.API.Models;

namespace InvestmentTracker.API.Validators;

public static class TransactionImportValidator
{
    public static ValidationResult<List<BulkUpsertItem>> Validate(
        List<TransactionImportRow> rows)
    {
        var result = new ValidationResult<List<BulkUpsertItem>>
        {
            Value = new List<BulkUpsertItem>()
        };

        foreach (var row in rows)
        {
            // Asset not resolved
            if (row.AssetId is null)
            {
                result.Errors.Add(new ValidationError
                {
                    Row = row.RowNumber,
                    Field = "Asset",
                    Message = $"Asset '{row.Asset}' not found"
                });
                continue;
            }

            if (row.Date is null)
            {
                result.Errors.Add(new ValidationError
                {
                    Row = row.RowNumber,
                    Field = "Date",
                    Message = "Date is required"
                });
                continue;
            }

            if (row.Date.Value.Date > DateTime.UtcNow.Date.AddDays(1))
            {
                result.Errors.Add(new ValidationError
                {
                    Row = row.RowNumber,
                    Field = "Date",
                    Message = "Date cannot be in the future"
                });
                continue;
            }

            if (row.IsEmpty && row.AssetStatus == AssetStatus.Inactive)
                continue;

            // Active + empty → skip (keep previous values)
            if (row.IsEmpty && row.AssetStatus == AssetStatus.Active)
                continue;

            // Partial row is NOT allowed
            if (row.ValueChange is null ||
                row.CurrentValue is null ||
                row.Dividend is null)
            {
                result.Errors.Add(new ValidationError
                {
                    Row = row.RowNumber,
                    Field = "Row",
                    Message = "Partial row detected. Fill all numeric fields or leave all empty."
                });
                continue;
            }

            ValidateNonNegative(result, row.RowNumber, "CurrentValue", row.CurrentValue.Value);
            ValidateNonNegative(result, row.RowNumber, "Dividend", row.Dividend.Value);

            if (!result.IsValid)
                continue;

            result.Value!.Add(new BulkUpsertItem
            {
                AssetId = row.AssetId.Value,
                ValueChange = row.ValueChange.Value,
                CurrentValue = row.CurrentValue.Value,
                Dividend = row.Dividend.Value,
                Notes = row.Notes
            });
        }

        return result;
    }

    private static void ValidateNonNegative(
        ValidationResult<List<BulkUpsertItem>> result,
        int row,
        string field,
        decimal value)
    {
        if (value < 0)
        {
            result.Errors.Add(new ValidationError
            {
                Row = row,
                Field = field,
                Message = "Value cannot be negative"
            });
        }
    }
}
