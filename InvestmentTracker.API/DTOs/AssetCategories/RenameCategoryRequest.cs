namespace InvestmentTracker.API.DTOs.AssetCategories;

public sealed class RenameCategoryRequest
{
    public string Name { get; set; } = default!;
    public bool? IsActive { get; set; }
}
