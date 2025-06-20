using AuctionService.Data;
using AuctionService.Entities.Enums;
using Contracts.Events;
using MassTransit;

namespace AuctionService.Consumers;
public class AuctionFinishedConsumer(AuctionDataContext dataContext) : IConsumer<AuctionFinished>
{
    private readonly AuctionDataContext _dataContext = dataContext;

    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        var message = context.Message;

        if (!Guid.TryParse(message.AuctionId, out var auctionId))
        {
            Console.WriteLine($"[WARN] Invalid Auction ID: '{message.AuctionId}'");
            return;
        }

        var auction = await _dataContext.Auctions.FindAsync(auctionId);
        if (auction is null)
        {
            Console.WriteLine($"[WARN] Auction not found: {auctionId}");
            return;
        }

        if (message.ItemSold)
        {
            if (string.IsNullOrWhiteSpace(message.Winner))
            {
                Console.WriteLine($"[WARN] Winner is missing for sold auction: {auctionId}");
                return;
            }

            if (!message.Amount.HasValue)
            {
                Console.WriteLine($"[WARN] Sold amount is missing for auction: {auctionId}");
                return;
            }

            auction.Winner = message.Winner;
            auction.SoldAmount = message.Amount.Value;

            auction.Status = auction.SoldAmount > auction.ReservePrice
                ? Status.Finished
                : Status.ReserveNotMet;
        }
        else
        {
            auction.Status = Status.ReserveNotMet;
        }

        _ = await _dataContext.SaveChangesAsync();

        Console.WriteLine($"[INFO] Auction updated | ID: {auctionId} | Status: {auction.Status}");
    }
}