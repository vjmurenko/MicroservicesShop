namespace HistoryService.Contract;

public interface ArchiveOrder
{
    public Guid CorrelationId { get; set; }
    public int TotalPrice { get; set; }
    public DateTimeOffset SubmitDate { get; set; }
    public string CurrentState { get; set; }
    
    public DateTimeOffset ConfirmDate { get; set; }
    public string Manager { get; set; }
    public string RejectionReason { get; set; }
    public DateTimeOffset RejectDate { get; set; }
}