public sealed class DashboardChartRequest
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }

    public required string GroupBy { get; set; }

    public List<int>? AssetIds { get; set; }
}
