using Contracts.Events;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        var item = new Item
        {
            Color = context.Message.Color,
            Make = context.Message.Make,
            Model = context.Message.Model,
            Mileage = context.Message.Mileage,
            Year = context.Message.Year
        };

        var result = await DB.Update<Item>()
            .Match(a => a.ID == context.Message.Id)
            .ModifyOnly(x => new { x.Color, x.Make, x.Model, x.Mileage, x.Year }, item)
            .ExecuteAsync();

        if (!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionUpdated), "Auction update failed");
        }
    }
}
