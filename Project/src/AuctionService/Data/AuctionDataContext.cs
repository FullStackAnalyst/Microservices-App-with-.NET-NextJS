using AuctionService.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class AuctionDataContext(DbContextOptions<AuctionDataContext> options) : DbContext(options)
{
    public DbSet<Auction> Auctions { get; set; }

    public DbSet<Item> Items { get; set; }
}
