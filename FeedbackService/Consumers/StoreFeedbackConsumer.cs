using FeedbackService.Database;
using FeedbackService.Database.Model;
using FeedbkackService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FeedbackService.Consumers;

public class StoreFeedbackConsumer : IConsumer<StoreFeedback>
{
    private readonly ILogger<StoreFeedbackConsumer> _logger;
    private readonly FeedbackDbContext _dbContext;

    public StoreFeedbackConsumer(ILogger<StoreFeedbackConsumer> logger, FeedbackDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<StoreFeedback> context)
    {
        _dbContext.Feebacks.Add(new Feedback {Message = context.Message.Message, Stars = context.Message.Stars, OrderId = context.Message.OrderId});
        await _dbContext.SaveChangesAsync();
        await context.RespondAsync<FeedbackSaved>(new { });
    }
}