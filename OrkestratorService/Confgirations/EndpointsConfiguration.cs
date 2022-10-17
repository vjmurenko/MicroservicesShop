namespace OrkestratorService.Confgirations;

public class EndpointsConfiguration
{
    public string CartServiceAddress { get; set; }
    public string DeliveryServiceAddress { get; set; }
    public string OrderStateMachineAddress { get; set; }
    public string PaymentServiceAddress { get; set; }
    public string HistoryServiceAddress { get; set; }
    public string FeedbackServiceAddress { get; set; }
}