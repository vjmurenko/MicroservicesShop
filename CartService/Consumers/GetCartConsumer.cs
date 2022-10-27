using CartService.Contracts;
using CartService.Database.Repository.Interfaces;
using MassTransit;
using Shared;

namespace CartService.Consumers;

public class GetCartConsumer : IConsumer<GetCart>
{
    private readonly ICartRepository _cartRepository;

    public GetCartConsumer(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task Consume(ConsumeContext<GetCart> context)
    {
        var cart = await _cartRepository.GetCartWithPositions(context.Message.Id);

        var cartContent = cart.CartPositions.Select(s => new CartPositionDto 
            {Amount = s.Amount, Name = s.Good.Name, Price = s.Good.Price});

        await context.RespondAsync<GetCartResponse>(new
        {
            OrderId = cart.Id,
            CartContent = cartContent,
            TotalPrice = cartContent.Sum(s => s.Price)
        });
    }
}