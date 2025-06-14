using Contracts.Events;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities.Models;

namespace SearchService.Consumers;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine($"Auction deleted: {context.Message.Id}");

        var result = await DB.DeleteAsync<Item>(context.Message.Id);

        if (!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionDeleted), "Failed to delete auction");
        }
    }
}
