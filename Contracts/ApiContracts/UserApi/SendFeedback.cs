namespace Api.Contracts.UserApi;

public interface SendFeedback
{
    public Guid OrderId { get; set; }
    public int Stars { get; set; }
    public string Message { get; set; }
}