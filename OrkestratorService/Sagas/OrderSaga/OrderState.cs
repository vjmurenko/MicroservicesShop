using Automatonymous;
using OrkestratorService.Database.Models;

namespace OrkestratorService.Sagas.OrderSaga;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public List<CartItem> CartItems { get; set; }
    public int TotalPrice { get; set; }
    public DateTimeOffset SubmitDate { get; set; }
    public string CurrentState { get; set; }
    
    public DateTimeOffset? ConfirmDate { get; set; }
    public string? Manager { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset? RejectDate { get; set; }
    
    public Guid? FeedbackTimeoutToken { get; set; }
    
    public byte[]? RowVersion { get; set; }
}