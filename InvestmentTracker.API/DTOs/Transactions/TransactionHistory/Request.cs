public class TransactionHistoryRequest
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }

    public int? AssetId { get; set; }   
    public int Page { get; set; }
    public int PageSize { get; set; }
}