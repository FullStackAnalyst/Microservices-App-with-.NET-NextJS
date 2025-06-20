using AuctionService.Data;
using Contracts.Events;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer(AuctionDataContext dataContext) : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        var message = context.Message;

        Console.WriteLine($"[INFO] Consuming BidPlaced: AuctionId={message.AuctionId}, Amount={message.Amount}, Status={message.BidStatus}");

        if (!Guid.TryParse(message.AuctionId, out var auctionId))
        {
            Console.WriteLine($"[WARN] Invalid AuctionId: '{message.AuctionId}'");
            return;
        }

        var auction = await dataContext.Auctions.FindAsync(auctionId);
        if (auction is null)
        {
            Console.WriteLine($"[WARN] Auction not found for ID: {auctionId}");
            return;
        }

        var isAccepted = message.BidStatus.Equals("Accepted", StringComparison.OrdinalIgnoreCase);
        var isHigher = auction.CurrentHighBid == null || message.Amount > auction.CurrentHighBid;

        if (isAccepted && isHigher)
        {
            auction.CurrentHighBid = message.Amount;
            _ = await dataContext.SaveChangesAsync();

            Console.WriteLine($"[INFO] Updated Auction {auctionId}: New High Bid = {auction.CurrentHighBid}");
        }
        else
        {
            Console.WriteLine($"[INFO] Bid ignored for Auction {auctionId}: Not accepted or lower than current high bid.");
        }
    }
}
