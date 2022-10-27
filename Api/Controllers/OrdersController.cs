using Api.Contracts.ManagerApi;
using Api.Contracts.UserApi;
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
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRoutingConfiguration _routingConfiguration;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        ISendEndpointProvider sendEndpointProvider,
        IPublishEndpoint publishEndpoint,
        IRoutingConfiguration routingConfiguration,
        ILogger<OrdersController> logger)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _publishEndpoint = publishEndpoint;
        _routingConfiguration = routingConfiguration;
        _logger = logger;
    }

    [HttpPost("{orderId}/cart-position/add")]
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

    [HttpPost("{orderId}/submit")]
    public async Task<IActionResult> SubmitOrder(Guid orderId)
    {
        try
        {
            await _publishEndpoint.Publish<SubmitOrder>(new
            {
                OrderId = orderId
            });
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500, e);
        }
    }

    [HttpPost("{orderId}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid orderId, string manager)
    {
        try
        {
            await _publishEndpoint.Publish<ConfirmOrder>(new
            {
                OrderId = orderId,
                Manager = manager
            });
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500, e);
        }
    }

    [HttpPost("{orderId}/reject")]
    public async Task<ActionResult> RejectOrder(Guid orderId, string rejectionReason, string manager)
    {
        try
        {
            await _publishEndpoint.Publish<RejectOrder>(new
            {
                OrderId = orderId,
                RejectionReason = rejectionReason,
                Manager = manager
            });
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode(500, e);
        }
    }

    [HttpPost("{orderId}/feedback")]
    public async Task<IActionResult> SendFeedback(Guid orderId, int stars, string message)
    {
        try
        {
            await _publishEndpoint.Publish<SendFeedback>(new
            {
                OrderId = orderId,
                Stars = stars,
                Message = message
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