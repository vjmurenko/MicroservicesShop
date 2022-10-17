namespace CartService.Contracts;

public interface AddCartPosition
{
    public Guid OrderId { get; set; }
    public string Name { get; set; }
    public int Amount { get; set; }
}