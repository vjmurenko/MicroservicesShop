namespace Api.Contracts.UserApi;

public class SubmitOrder
{
    public Guid OrderId { get; set; }
    public string UserName { get; set; }
}