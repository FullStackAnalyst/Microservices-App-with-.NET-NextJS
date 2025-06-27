using AuctionService.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Auction> Auctions { get; set; }
}
