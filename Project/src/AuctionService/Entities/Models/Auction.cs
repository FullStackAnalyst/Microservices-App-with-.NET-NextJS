﻿using AuctionService.Entities.Enums;

namespace AuctionService.Entities.Models;

public class Auction
{
    public Guid Id { get; set; }

    public int ReservePrice { get; set; } = 0;

    public string Seller { get; set; } = string.Empty;

    public string Winner { get; set; } = string.Empty;

    public int? SoldAmount { get; set; }

    public int? CurrentHighBid { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime AuctionEnd { get; set; }

    public Status Status { get; set; } = Status.Live;

    public Item Item { get; set; } = null!;
}