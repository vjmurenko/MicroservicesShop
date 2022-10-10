using CartService.Database.Models;
using CartService.Database.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartService.Database.Repository;

public class GoodRepository : IGoodRepository
{
    private readonly NpgSqlContext _dbContext;

    public GoodRepository(NpgSqlContext dbContext)
    {
        _dbContext = dbContext;
    }

    public  Task<bool> GoodExist(string name)
    {
        return  _dbContext.Goods.AsNoTracking().AnyAsync(g => g.Name == name);
    }

    public Task<Good> GetGoodByName(string name)
    {
        return _dbContext.Goods.AsNoTracking().FirstOrDefaultAsync(g => g.Name == name)!;
    }

    public async Task AddGood(Guid id, string name, int price)
    {
        var good = new Good(id, name, price);
        await _dbContext.Goods.AddAsync(good);
        await _dbContext.SaveChangesAsync();
    }
}