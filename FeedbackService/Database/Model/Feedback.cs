namespace FeedbackService.Database.Model;

public class Feedback
{
    public Guid OrderId { get; set; }
    public int Stars { get; set; }
    public string Message { get; set; }
}