namespace AuctionService.Entities.Models;

public class Item
{
    public Guid Id { get; set; }

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public string Color { get; set; } = string.Empty;

    public int Mileage { get; set; }

    public string ImageURL { get; set; } = string.Empty;

    public Auction Auction { get; set; } = null!;

    public Guid AuctionId { get; set; }
}
