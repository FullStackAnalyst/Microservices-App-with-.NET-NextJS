using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Entities.Models;
using SearchService.Services;

namespace SearchService.Data;

public static class Initializer
{
    public static async Task ConfigureDatabase(WebApplication app)
    {
        await DB.InitAsync("SearchDB", MongoClientSettings
            .FromConnectionString(app.Configuration.GetConnectionString("SCS")));

        try
        {
            _ = await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();
        }
        catch (MongoDB.Driver.MongoCommandException ex)
            when (ex.Message.Contains("index not found with name [[TEXT]]"))
        {
            // Ignore this error: it means the index didn't exist, which is fine for first run
        }

        var count = await DB.CountAsync<Item>();

        using var scope = app.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<AuctionSyncClient>();

        var items = await client.GetRecentAuctionListings();

        Console.WriteLine($"Number of items retrieved: {items.Count}");

        if (items.Count > 0)
        {
            _ = await DB.SaveAsync(items);
        }
    }
}