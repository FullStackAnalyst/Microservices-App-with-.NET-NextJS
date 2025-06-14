using Contracts.Events;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities.Models;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        var message = context.Message;
        Console.WriteLine($"AuctionCreated: {message.Id}");

        var item = new Item
        {
            ID = message.Id.ToString(),
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt,
            AuctionEnd = message.AuctionEnd,
            Seller = message.Seller,
            Winner = message.Winner,
            Make = message.Make,
            Model = message.Model,
            Year = message.Year,
            Color = message.Color,
            Mileage = message.Mileage,
            ImageURL = message.ImageURL,
            Status = message.Status,
            ReservePrice = message.ReservePrice,
            SoldAmount = message.SoldAmount == 0 ? null : message.SoldAmount,
            CurrentHighBid = message.CurrentHighBid == 0 ? null : message.CurrentHighBid
        };

        if (item.Model == "Foo")
        {
            throw new ArgumentException("Model cannot be Foo");
        }

        await item.SaveAsync();
    }
}