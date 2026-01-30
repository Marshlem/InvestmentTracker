using ClosedXML.Excel;
using InvestmentTracker.API.DTOs.TransactionsImport;

namespace InvestmentTracker.API.Infrastructure.Imports.Excel;

public sealed class ExcelTransactionImportReader
{
    public List<TransactionImportRow> Read(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        if (worksheet == null)
            throw new InvalidOperationException("Excel worksheet not found");

        var rows = new List<TransactionImportRow>();

        var firstDataRow = 2;
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = firstDataRow; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);

            if (row.CellsUsed().All(c => c.IsEmpty()))
                continue;

            rows.Add(new TransactionImportRow
            {
                Date = row.Cell(1).IsEmpty()
                ? null
                : row.Cell(1).GetDateTime(),

                RowNumber = rowNumber,

                Asset = row.Cell(2).GetString().Trim(),

                ValueChange = TryGetDecimal(row.Cell(3)),
                CurrentValue = TryGetDecimal(row.Cell(4)),
                Dividend = TryGetDecimal(row.Cell(5)),

                Notes = row.Cell(6).IsEmpty()
                    ? null
                    : row.Cell(6).GetString()
            });
        }

        return rows;
    }

    private static decimal? TryGetDecimal(IXLCell cell)
    {
        if (cell.IsEmpty())
            return null;

        if (cell.TryGetValue<decimal>(out var value))
            return value;

        var str = cell.GetString();
        if (decimal.TryParse(str, out var parsed))
            return parsed;

        throw new FormatException(
            $"Invalid decimal value '{cell.GetString()}' at {cell.Address}");
    }
}
