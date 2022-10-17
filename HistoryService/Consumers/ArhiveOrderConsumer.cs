using HistoryService.Contract;
using HistoryService.Database;
using HistoryService.Database.Models;
using MassTransit;

namespace HistoryService.Consumers;

public class ArhiveOrderConsumer : IConsumer<ArchiveOrder>
{
    private readonly HistoryDbContext _dbContext;

    public ArhiveOrderConsumer(HistoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Consume(ConsumeContext<ArchiveOrder> context)
    {
        _dbContext.Orders.Add(new Order
        {
            OrderId = context.Message.CorrelationId,
            TotalPrice = context.Message.TotalPrice,
            Manager = context.Message.Manager,
            ConfirmDate = context.Message.ConfirmDate,
            CurrentState = context.Message.CurrentState,
            RejectionReason = context.Message.RejectionReason,
            SubmitDate = context.Message.SubmitDate,
            RejectDate = context.Message.RejectDate
        });

        await _dbContext.SaveChangesAsync();
        await context.RespondAsync<OrderArchived>(new { });
    }
}