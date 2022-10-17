namespace Api.Contracts.ManagerApi;

public interface ConfirmOrder
{
    public Guid OrderId { get; set; }
    public string Manager { get; set; }
}