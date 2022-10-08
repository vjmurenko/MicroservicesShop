namespace CartService.Database.Repository.Interfaces;

public interface ICartPositionRepository
{
    public Task AddCartPosition(Guid id, Guid cartId, Guid goodId, int amount);
    public Task UpdateCartPosition(Guid id, Guid cartId, Guid goodId, int amount);
    public Task<bool> CartPositionExistsForCart(Guid id, string name);
    public Task RemoveCartPosition(Guid id);
}