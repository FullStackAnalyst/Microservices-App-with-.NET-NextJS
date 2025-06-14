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

        var requestUrl = $"{auctionServiceUrl}/auction/auctions?date={lastUpdated}";

        try
        {
            var response = await _client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<List<Item>>();
                return items ?? [];
            }
            else
            {
                Console.WriteLine($"AuctionService returned status {(int)response.StatusCode}: {response.ReasonPhrase}");
                return [];
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request failed: {ex.Message}");
            return [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return [];
        }
    }
}