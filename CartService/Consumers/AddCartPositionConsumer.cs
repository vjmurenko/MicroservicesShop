using CartService.Contracts;
using CartService.Database.Repository.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CartService.Consumers;

public class AddCartPositionConsumer : IConsumer<AddCartPosition>
{
    private readonly ILogger<AddCartPositionConsumer> _logger;
    private readonly ICartRepository _cartRepository;
    private readonly ICartPositionRepository _cartPositionRepository;
    private readonly IGoodRepository _goodRepository;
    private readonly Random _random;

    public AddCartPositionConsumer(
        ILogger<AddCartPositionConsumer> logger,
        ICartRepository cartRepository,
        ICartPositionRepository cartPositionRepository,
        IGoodRepository goodRepository
    )
    {
        _logger = logger;
        _cartRepository = cartRepository;
        _cartPositionRepository = cartPositionRepository;
        _goodRepository = goodRepository;
        _random = new Random();
    }

    public async Task Consume(ConsumeContext<AddCartPosition> context)
    {
        var newCartPosition = context.Message;
        if (!await _cartRepository.IsCartExist(newCartPosition.OrderId))
        {
            await _cartRepository.AddCart(newCartPosition.OrderId);
        }

        if (!await _goodRepository.GoodExist(newCartPosition.Name))
        {
            await _goodRepository.AddGood(Guid.NewGuid(), newCartPosition.Name, _random.Next(100, 1500));
        }

        var cart = await _cartRepository.GetCartWithPositions(newCartPosition.OrderId);
        var cartPosition = cart.CartPositions.FirstOrDefault(c => c.Good.Name == newCartPosition.Name);
        if (cartPosition != null)
        {
            await _cartPositionRepository.UpdateCartPosition(cartPosition.Id, cart.Id, cartPosition.GoodId, cartPosition.Amount + newCartPosition.Amount);
        }
        else
        {
            var good = await _goodRepository.GetGoodByName(newCartPosition.Name);
            await _cartPositionRepository.AddCartPosition(Guid.NewGuid(), newCartPosition.OrderId, good.Id, newCartPosition.Amount);
        }
    }
}