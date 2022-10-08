using CartService.Database.Models;
using CartService.Database.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartService.Database.Repository;

public class CartPositionRepository : ICartPositionRepository
{
    private readonly NpgSqlContext _dbContext;

    public CartPositionRepository(NpgSqlContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddCartPosition(Guid id, Guid cartId, Guid goodId, int amount)
    {
        var cardPosition = new CartPosition(id, cartId, goodId, amount);
        await _dbContext.CartPositions.AddAsync(cardPosition);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCartPosition(Guid id, Guid cartId, Guid goodId, int amount)
    {
        var cartPosition = await _dbContext.CartPositions.FirstOrDefaultAsync(s => s.Id == id);
        if (cartPosition != null)
        {
            cartPosition.Id = id;
            cartPosition.CartId = cartId;
            cartPosition.GoodId = goodId;
            cartPosition.Amount = amount;
        }

        await _dbContext.SaveChangesAsync();
    }

    public Task<bool> CartPositionExistsForCart(Guid id, string name)
    {
        return _dbContext.CartPositions.Include(s => s.Good).AsNoTracking().AnyAsync(s => s.Id == id && s.Good.Name == name);
    }

    public async Task RemoveCartPosition(Guid id)
    {
        var cartPosition = await _dbContext.CartPositions.FirstOrDefaultAsync(s => s.Id == id);
        _dbContext.Remove(cartPosition);
        await _dbContext.SaveChangesAsync();
    }
}