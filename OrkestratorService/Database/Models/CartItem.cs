using OrkestratorService.Sagas.OrderSaga;

namespace OrkestratorService.Database.Models;

public class CartItem
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public int Amount { get; set; }
    public int Price { get; set; }
    
    public Guid OrderId { get; set; }
    public OrderState OrderState { get; set; }
}