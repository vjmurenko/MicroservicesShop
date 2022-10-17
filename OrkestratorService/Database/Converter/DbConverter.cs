using OrkestratorService.Database.Models;
using Shared;

namespace OrkestratorService.Database.Converter;

public static class DbConverter
{
    public static List<CartItem> ConvertToCartItemsList(List<CartPositionDto> cartPositionDtos, Guid orderId)
    {
        return cartPositionDtos.Select(s => new CartItem {Id = Guid.NewGuid(), OrderId = orderId, Amount = s.Amount, Name = s.Name, Price = s.Price})
            .ToList();
    }

    public static List<CartPositionDto> ConvertToCartPositionDtoList(List<CartItem> cartItems)
    {
        return cartItems.Select(item => new CartPositionDto() {Amount = item.Amount, Name = item.Name, Price = item.Price}).ToList();
    }
}