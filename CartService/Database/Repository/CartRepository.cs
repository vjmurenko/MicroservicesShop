using CartService.Database.Models;
using CartService.Database.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartService.Database.Repository;

public class CartRepository : ICartRepository
{
    private readonly NpgSqlContext _dbContext;

    public CartRepository(NpgSqlContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddCart(Guid id)
    {
        var cart = new Cart(id);
        await _dbContext.Carts.AddAsync(cart);
        await _dbContext.SaveChangesAsync();
    }

    public Task<bool> IsCartExist(Guid id)
    {
        return _dbContext.Carts.AnyAsync(s => s.Id == id);
    }

    public Task<Cart> GetCartWithPositions(Guid id)
    {
        return _dbContext.Carts
            .Include(c => c.CartPositions)
            .ThenInclude(c => c.Good)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id)!;
    }
}