namespace Api.Contracts.ManagerApi;

public interface RejectOrder
{
    public Guid OrderId { get; set; }
    public string RejectionReason { get; set; }
    public string Manager { get; set; }
}