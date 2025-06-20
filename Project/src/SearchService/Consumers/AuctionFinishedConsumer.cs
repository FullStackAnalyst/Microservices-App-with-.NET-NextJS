using Contracts.Events;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities.Models;

namespace SearchService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine("--> Consuming auction finished event");

        var auctionId = context.Message.AuctionId;

        var auction = await DB.Find<Item>().OneAsync(auctionId);

        if (auction == null)
        {
            Console.WriteLine($"[WARN] Auction not found for ID: {auctionId}");
            return;
        }

        if (context.Message.ItemSold)
        {
            if (!string.IsNullOrWhiteSpace(context.Message.Winner))
            {
                auction.Winner = context.Message.Winner;
            }

            auction.SoldAmount = context.Message.Amount ?? 0;
        }

        auction.Status = "Finished";

        await auction.SaveAsync();

        Console.WriteLine($"[INFO] Auction {auctionId} marked as Finished");
    }
}