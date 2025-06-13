using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Entities.Models;
using System.Text.Json;

namespace SearchService.Data;

public static class Initializer
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task InitializeDB(WebApplication app)
    {
        await InitializeMongoDB(app);
        var coll = GetCollection<Item>("SearchDB", "Item");

        var count = await GetDocumentCountAsync(coll);
        Console.WriteLine($"Current 'Item' count: {count}");

        if (count == 0)
        {
            await SeedDatabase(coll);
        }
        else
        {
            Console.WriteLine("Items already exist. Skipping seeding.");
        }
    }

    private static async Task InitializeMongoDB(WebApplication app)
    {
        var connectionString = app.Configuration.GetConnectionString("MCS");
        await DB.InitAsync("SearchDB", MongoClientSettings.FromConnectionString(connectionString));
        Console.WriteLine("Connected to MongoDB");
    }

    private static IMongoCollection<T> GetCollection<T>(string databaseName, string collectionName)
    {
        var db = DB.Database(databaseName);
        return db.GetCollection<T>(collectionName);
    }

    private static async Task<long> GetDocumentCountAsync(IMongoCollection<Item> collection)
    {
        return await collection.CountDocumentsAsync(FilterDefinition<Item>.Empty);
    }

    private static async Task SeedDatabase(IMongoCollection<Item> collection)
    {
        var dataFile = Path.Combine("Data", "data.json");

        if (!File.Exists(dataFile))
        {
            Console.WriteLine($"data.json not found at: {dataFile}");
            return;
        }

        var items = await LoadJsonDataAsync(dataFile);
        if (items is null || items.Count == 0)
        {
            Console.WriteLine("No items found in data.json");
            return;
        }

        await collection.InsertManyAsync(items);
        Console.WriteLine($"{items.Count} items inserted into MongoDB.");
    }

    private static async Task<List<Item>?> LoadJsonDataAsync(string filePath)
    {
        try
        {
            var data = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<Item>>(data, _options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during data import: {ex.Message}");
            return null;
        }
    }
}