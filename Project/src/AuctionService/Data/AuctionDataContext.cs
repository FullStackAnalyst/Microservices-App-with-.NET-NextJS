using AuctionService.Entities.Enums;
using AuctionService.Entities.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AuctionService.Data;

public class AuctionDataContext(DbContextOptions<AuctionDataContext> options) : DbContext(options)
{
    public DbSet<Auction> Auctions { get; set; }
    public DbSet<Item> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        _ = modelBuilder.Entity<Auction>()
            .HasOne(a => a.Item)
            .WithOne(i => i.Auction)
            .HasForeignKey<Item>(i => i.AuctionId);

        _ = modelBuilder.Entity<Auction>().HasData(
            new Auction
            {
                Id = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c"),
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Auction
            {
                Id = Guid.Parse("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"),
                Status = Status.Live,
                ReservePrice = 90000,
                Seller = "alice",
                AuctionEnd = DateTime.UtcNow.AddDays(60),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Auction
            {
                Id = Guid.Parse("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"),
                Status = Status.Live,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(4),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Auction
            {
                Id = Guid.Parse("155225c1-4448-4066-9886-6786536e05ea"),
                Status = Status.ReserveNotMet,
                ReservePrice = 50000,
                Seller = "tom",
                AuctionEnd = DateTime.UtcNow.AddDays(-10),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Auction
            {
                Id = Guid.Parse("466e4744-4dc5-4987-aae0-b621acfc5e39"),
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "alice",
                AuctionEnd = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Auction
            {
                Id = Guid.Parse("dc1e4071-d19d-459b-b848-b5c3cd3d151f"),
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(45),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Auction
            {
                Id = Guid.Parse("47111973-d176-4feb-848d-0ea22641c31a"),
                Status = Status.Live,
                ReservePrice = 150000,
                Seller = "alice",
                AuctionEnd = DateTime.UtcNow.AddDays(13),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Auction
            {
                Id = Guid.Parse("6a5011a1-fe1f-47df-9a32-b5346b289391"),
                Status = Status.Live,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(19),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Auction
            {
                Id = Guid.Parse("40490065-dac7-46b6-acc4-df507e0d6570"),
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "tom",
                AuctionEnd = DateTime.UtcNow.AddDays(20),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Auction
            {
                Id = Guid.Parse("3659ac24-29dd-407a-81f5-ecfe6f924b9b"),
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(48),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        _ = modelBuilder.Entity<Item>().HasData(
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "Ford",
                Model = "GT",
                Year = 2020,
                Color = "White",
                Mileage = 50000,
                ImageURL = "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg",
                AuctionId = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c")
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "Bugatti",
                Model = "Veyron",
                Year = 2018,
                Color = "Black",
                Mileage = 15035,
                ImageURL = "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg",
                AuctionId = Guid.Parse("c8c3ec17-01bf-49db-82aa-1ef80b833a9f")
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "Ford",
                Model = "Mustang",
                Year = 2023,
                Color = "Black",
                Mileage = 65125,
                ImageURL = "https://cdn.pixabay.com/photo/2012/11/02/13/02/car-63930_960_720.jpg",
                AuctionId = Guid.Parse("bbab4d5a-8565-48b1-9450-5ac2a5c4a654")
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "Mercedes",
                Model = "SLK",
                Year = 2020,
                Color = "Silver",
                Mileage = 15001,
                ImageURL = "https://cdn.pixabay.com/photo/2016/04/17/22/10/mercedes-benz-1335674_960_720.png",
                AuctionId = Guid.Parse("155225c1-4448-4066-9886-6786536e05ea")
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "BMW",
                Model = "X1",
                Year = 2017,
                Color = "White",
                Mileage = 90000,
                ImageURL = "https://cdn.pixabay.com/photo/2017/08/31/05/47/bmw-2699538_960_720.jpg",
                AuctionId = Guid.Parse("466e4744-4dc5-4987-aae0-b621acfc5e39")
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "Ferrari",
                Model = "Spider",
                Year = 2015,
                Color = "Red",
                Mileage = 50000,
                ImageURL = "https://cdn.pixabay.com/photo/2017/11/09/01/49/ferrari-458-spider-2932191_960_720.jpg",
                AuctionId = Guid.Parse("dc1e4071-d19d-459b-b848-b5c3cd3d151f")
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "Ferrari",
                Model = "F-430",
                Year = 2022,
                Color = "Red",
                Mileage = 5000,
                ImageURL = "https://cdn.pixabay.com/photo/2017/11/08/14/39/ferrari-f430-2930661_960_720.jpg",
                AuctionId = Guid.Parse("47111973-d176-4feb-848d-0ea22641c31a")
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "Audi",
                Model = "R8",
                Year = 2021,
                Color = "White",
                Mileage = 10050,
                ImageURL = "https://cdn.pixabay.com/photo/2019/12/26/20/50/audi-r8-4721217_960_720.jpg",
                AuctionId = Guid.Parse("6a5011a1-fe1f-47df-9a32-b5346b289391")
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "Audi",
                Model = "TT",
                Year = 2020,
                Color = "Black",
                Mileage = 25400,
                ImageURL = "https://cdn.pixabay.com/photo/2016/09/01/15/06/audi-1636320_960_720.jpg",
                AuctionId = Guid.Parse("40490065-dac7-46b6-acc4-df507e0d6570")
            },
            new Item
            {
                Id = Guid.NewGuid(),
                Make = "Ford",
                Model = "Model T",
                Year = 1938,
                Color = "Rust",
                Mileage = 150150,
                ImageURL = "https://cdn.pixabay.com/photo/2017/08/02/19/47/vintage-2573090_960_720.jpg",
                AuctionId = Guid.Parse("3659ac24-29dd-407a-81f5-ecfe6f924b9b")
            }
        );
    }
}