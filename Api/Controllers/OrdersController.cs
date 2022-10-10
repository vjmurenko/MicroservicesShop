using Api.Models.Interfaces;
using CartService.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IRoutingConfiguration _routingConfiguration;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        ISendEndpointProvider sendEndpointProvider,
        IRoutingConfiguration routingConfiguration,
        ILogger<OrdersController> logger)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _routingConfiguration = routingConfiguration;
        _logger = logger;
    }


    [HttpPost]
    [Route("{orderId}/cart-position/add")]
    public async Task<IActionResult> AddCartPosition(Guid orderId, string name, int amount)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(_routingConfiguration.CartServiceAddress!));
        try
        {
            await sendEndpoint.Send<AddCartPosition>(new
            {
                OrderId = orderId,
                Name = name,
                Amount = amount
            });
            return Ok();

        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500, e);
        }
    }
}