namespace FeedbkackService.Contracts;

public interface StoreFeedback
{
    public Guid OrderId { get; set; }
    public int Stars { get; set; }
    public string Message { get; set; }
}