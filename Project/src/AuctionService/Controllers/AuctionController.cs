using AuctionService.Data;
using AuctionService.Entities.DTOs;
using AuctionService.Entities.Enums;
using AuctionService.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[Route("[controller]")]
[ApiController]
public class AuctionController(AuctionDataContext context) : ControllerBase
{
    private readonly AuctionDataContext _context = context;

    [Route("auctions")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAuctions()
    {
        List<AuctionDto> auctions = await (from a in _context.Auctions
                                           join i in _context.Items
                                           on a.Id equals i.AuctionId
                                           select new AuctionDto
                                           {
                                               Id = a.Id,
                                               CreatedAt = a.CreatedAt,
                                               UpdatedAt = a.UpdatedAt,
                                               AuctionEnd = a.AuctionEnd,
                                               Seller = a.Seller,
                                               Winner = a.Winner,
                                               Status = a.Status.ToString(),
                                               ReservePrice = a.ReservePrice,
                                               SoldAmount = a.SoldAmount,
                                               CurrentHighBid = a.CurrentHighBid,
                                               Make = i.Make,
                                               Model = i.Model,
                                               Year = i.Year,
                                               Color = i.Color,
                                               Mileage = i.Mileage,
                                               ImageURL = i.ImageURL
                                           })
                                           .AsNoTracking()
                                           .ToListAsync();

        return Ok(auctions);
    }

    [Route("auction")]
    [HttpGet]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid auctionId)
    {
        AuctionDto? auction = await (from a in _context.Auctions
                                     join i in _context.Items on a.Id equals i.AuctionId
                                     where a.Id == auctionId
                                     select new AuctionDto
                                     {
                                         Id = a.Id,
                                         CreatedAt = a.CreatedAt,
                                         UpdatedAt = a.UpdatedAt,
                                         AuctionEnd = a.AuctionEnd,
                                         Seller = a.Seller,
                                         Winner = a.Winner,
                                         Status = a.Status.ToString(),
                                         ReservePrice = a.ReservePrice,
                                         SoldAmount = a.SoldAmount,
                                         CurrentHighBid = a.CurrentHighBid,
                                         Make = i.Make,
                                         Model = i.Model,
                                         Year = i.Year,
                                         Color = i.Color,
                                         Mileage = i.Mileage,
                                         ImageURL = i.ImageURL
                                     })
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync();

        return auction == null ? NotFound() : Ok(auction);
    }

    [HttpPost("add")]
    public async Task<ActionResult<AuctionDto>> PostAuction(CreateAuctionDto createAuctionDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string sellerUsername = User?.Identity?.Name ?? "unknown";

        Auction auction = new()
        {
            Id = Guid.NewGuid(),
            Seller = sellerUsername,
            ReservePrice = createAuctionDto.ReservePrice,
            AuctionEnd = createAuctionDto.AuctionEnd,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = Status.Live,
            Item = new Item
            {
                Id = Guid.NewGuid(),
                Make = createAuctionDto.Make,
                Model = createAuctionDto.Model,
                Year = createAuctionDto.Year,
                Color = createAuctionDto.Color,
                Mileage = createAuctionDto.Mileage,
                ImageURL = createAuctionDto.ImageUrl,
                AuctionId = Guid.Empty
            }
        };

        _ = await _context.Auctions.AddAsync(auction);
        _ = await _context.SaveChangesAsync();

        AuctionDto auctionDto = new()
        {
            Id = auction.Id,
            CreatedAt = auction.CreatedAt,
            UpdatedAt = auction.UpdatedAt,
            AuctionEnd = auction.AuctionEnd,
            Seller = auction.Seller,
            Winner = auction.Winner,
            Make = auction.Item.Make,
            Model = auction.Item.Model,
            Year = auction.Item.Year,
            Color = auction.Item.Color,
            Mileage = auction.Item.Mileage,
            ImageURL = auction.Item.ImageURL,
            Status = auction.Status.ToString(),
            ReservePrice = auction.ReservePrice,
            SoldAmount = auction.SoldAmount,
            CurrentHighBid = auction.CurrentHighBid
        };

        return CreatedAtAction(nameof(GetAuction), new { auctionId = auction.Id }, auctionDto);
    }

    [Route("update")]
    [HttpPut]
    public async Task<IActionResult> PutAuction(Guid auctionId, UpdateAuctionDto updateAuctionDto)
    {
        var result = await _context.Auctions
            .Where(a => a.Id == auctionId)
            .Select(a => new
            {
                AuctionId = a.Id,
                ItemId = a.Item.Id
            })
            .FirstOrDefaultAsync();

        if (result == null)
        {
            return NotFound();
        }

        Auction auction = new() { Id = result.AuctionId };
        _ = _context.Attach(auction);
        auction.UpdatedAt = DateTime.UtcNow;

        Item item = new() { Id = result.ItemId };
        _ = _context.Attach(item);

        if (!string.IsNullOrWhiteSpace(updateAuctionDto.Make))
        {
            item.Make = updateAuctionDto.Make;
        }

        if (!string.IsNullOrWhiteSpace(updateAuctionDto.Model))
        {
            item.Model = updateAuctionDto.Model;
        }

        if (!string.IsNullOrWhiteSpace(updateAuctionDto.Color))
        {
            item.Color = updateAuctionDto.Color;
        }

        if (updateAuctionDto.Year.HasValue)
        {
            item.Year = updateAuctionDto.Year.Value;
        }

        if (updateAuctionDto.Mileage.HasValue)
        {
            item.Mileage = updateAuctionDto.Mileage.Value;
        }

        _ = await _context.SaveChangesAsync();
        return NoContent();
    }

    [Route("delete")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAuction(Guid auctionId)
    {
        int deletedCount = await _context.Auctions
            .Where(a => a.Id == auctionId)
            .ExecuteDeleteAsync();

        return deletedCount == 0 ? NotFound() : NoContent();
    }
}
