using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Entities.Models;

namespace SearchService.Controllers;
[Route("[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<object>> SearchItems(string searchTerm, int pageNumber = 1, int pageSize = 5)
    {
        var collection = DB.Collection<Item>();

        var filter = Builders<Item>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            filter = Builders<Item>.Filter.Or(
                Builders<Item>.Filter.Regex(x => x.Make, new BsonRegularExpression(searchTerm, "i")),
                Builders<Item>.Filter.Regex(x => x.Model, new BsonRegularExpression(searchTerm, "i")),
                Builders<Item>.Filter.Regex(x => x.Color, new BsonRegularExpression(searchTerm, "i"))
            );
        }

        var totalCount = await collection.CountDocumentsAsync(filter);

        var items = await collection
            .Find(filter)
            .Sort(Builders<Item>.Sort.Ascending(x => x.Make))
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return Ok(new
        {
            results = items,
            pageCount = (int)Math.Ceiling((double)totalCount / pageSize),
            totalCount
        });
    }
}