using Shared;

namespace CartService.Contracts;

public interface GetCartResponse
{
    public Guid OrderId { get; set; }
    public List<CartPositionDto> CartContent { get; set; }
    public int TotalPrice { get; set; }
}