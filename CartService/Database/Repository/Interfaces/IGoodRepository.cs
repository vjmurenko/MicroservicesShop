using CartService.Database.Models;

namespace CartService.Database.Repository.Interfaces;

public interface IGoodRepository
{
    public Task<bool> GoodExist(string name);
    public Task<Good> GetGoodByName(string name);
    public Task AddGood(Guid id, string name, int price);
}