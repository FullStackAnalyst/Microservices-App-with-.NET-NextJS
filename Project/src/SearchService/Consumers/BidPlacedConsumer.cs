using Contracts.Events;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities.Models;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("--> Consuming bid placed");

        var auctionId = context.Message.AuctionId;

        var auction = await DB.Find<Item>().OneAsync(auctionId);

        if (auction == null)
        {
            Console.WriteLine($"[WARN] Auction not found for ID: {auctionId}");
            return;
        }

        var isAccepted = context.Message.BidStatus?.IndexOf("Accepted", StringComparison.OrdinalIgnoreCase) >= 0;
        var currentHighBid = auction.CurrentHighBid ?? 0;

        if (currentHighBid == 0 || (isAccepted && context.Message.Amount > currentHighBid))
        {
            auction.CurrentHighBid = context.Message.Amount;
            await auction.SaveAsync();

            Console.WriteLine($"[INFO] Updated Auction {auctionId} CurrentHighBid to {auction.CurrentHighBid}");
        }
        else
        {
            Console.WriteLine($"[INFO] Bid ignored for Auction {auctionId}: Bid not accepted or lower than current high bid.");
        }
    }
}