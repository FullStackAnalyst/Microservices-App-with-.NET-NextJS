using MongoDB.Entities;
using SearchService.Entities.Models;

namespace SearchService.Services;

public class AuctionSyncClient(HttpClient client, IConfiguration config)
{
    private readonly HttpClient _client = client;
    private readonly IConfiguration _config = config;

    public async Task<List<Item>> GetRecentAuctionListings()
    {
        var lastUpdatedItem = await DB.Find<Item>()
            .Sort(x => x.Descending(y => y.UpdatedAt))
            .ExecuteFirstAsync();

        var lastUpdated = lastUpdatedItem?.UpdatedAt.ToString("o") ?? string.Empty;

        var auctionServiceUrl = _config["AuctionServiceURL"];
        if (string.IsNullOrWhiteSpace(auctionServiceUrl))
        {
            return [];
        }

        Console.WriteLine($"Requesting auction data from: {auctionServiceUrl}/auction/auctions?date={lastUpdated}");

        try
        {
            var response = await _client.GetAsync($"{auctionServiceUrl}/auction/auctions?date={lastUpdated}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"AuctionService returned status {(int)response.StatusCode}: {response.ReasonPhrase}");
                return [];
            }

            var items = await response.Content.ReadFromJsonAsync<List<Item>>();

            return items ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching auction listings: {ex.Message}");
            return [];
        }
    }
}