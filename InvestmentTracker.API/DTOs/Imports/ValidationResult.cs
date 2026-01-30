namespace InvestmentTracker.API.DTOs.Imports;

public sealed class ValidationResult<T>
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = new();
    public T? Value { get; set; }
}