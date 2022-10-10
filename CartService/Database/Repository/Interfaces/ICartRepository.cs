using CartService.Database.Models;

namespace CartService.Database.Repository.Interfaces;

public interface ICartRepository
{
    public Task AddCart(Guid id);
    public Task<bool> IsCartExist(Guid id);
    public Task<Cart> GetCartWithPositions(Guid id);
}