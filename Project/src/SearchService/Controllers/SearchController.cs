using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Entities.Models;
using SearchService.Entities.Requests;

namespace SearchService.Controllers;
[Route("[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    [Route("search")]
    [HttpGet]
    public async Task<ActionResult<object>> SearchItems([FromQuery] PagedSearchRequest request)
    {
        var collection = DB.Collection<Item>();

        var filters = new List<FilterDefinition<Item>>();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            filters.Add(Builders<Item>.Filter.Or(
                Builders<Item>.Filter.Regex(x => x.Make, new BsonRegularExpression(request.SearchTerm, "i")),
                Builders<Item>.Filter.Regex(x => x.Model, new BsonRegularExpression(request.SearchTerm, "i")),
                Builders<Item>.Filter.Regex(x => x.Color, new BsonRegularExpression(request.SearchTerm, "i"))
            ));
        }

        if (!string.IsNullOrWhiteSpace(request.Seller))
        {
            filters.Add(Builders<Item>.Filter.Eq(x => x.Seller, request.Seller));
        }

        if (!string.IsNullOrWhiteSpace(request.Winner))
        {
            filters.Add(Builders<Item>.Filter.Eq(x => x.Winner, request.Winner));
        }

        var currentTime = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(request.FilterBy))
        {
            filters.Add(request.FilterBy.ToLower() switch
            {
                "finished" => Builders<Item>.Filter.Lt(x => x.AuctionEnd, currentTime),
                "endingsoon" => Builders<Item>.Filter.And(
                    Builders<Item>.Filter.Lt(x => x.AuctionEnd, currentTime.AddHours(6)),
                    Builders<Item>.Filter.Gt(x => x.AuctionEnd, currentTime)
                ),
                _ => Builders<Item>.Filter.Gt(x => x.AuctionEnd, currentTime)
            });
        }

        var filter = filters.Count > 0 ? Builders<Item>.Filter.And(filters) : Builders<Item>.Filter.Empty;

        var totalCount = await collection.CountDocumentsAsync(filter);

        var query = collection.Find(filter);

        query = request.OrderBy?.ToLower() switch
        {
            "make" => query.Sort(Builders<Item>.Sort.Ascending(x => x.Make).Ascending(x => x.Model)),
            "new" => query.Sort(Builders<Item>.Sort.Descending(x => x.CreatedAt)),
            _ => query.Sort(Builders<Item>.Sort.Ascending(x => x.AuctionEnd))
        };

        var items = await query
            .Skip(Math.Max(0, (request.PageNumber - 1) * request.PageSize))
            .Limit(request.PageSize)
            .ToListAsync();

        return Ok(new
        {
            results = items,
            pageCount = (int)Math.Ceiling((double)totalCount / request.PageSize),
            totalCount
        });
    }
}