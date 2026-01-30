namespace InvestmentTracker.API.DTOs.Imports;

public sealed class ValidationError
{
    public int Row { get; set; }
    public string Field { get; set; } = "";
    public string Message { get; set; } = "";
}