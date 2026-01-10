public sealed class DashboardChartRequest
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public string GroupBy { get; set; } = "month";

    public List<int>? AssetIds { get; set; }
}
