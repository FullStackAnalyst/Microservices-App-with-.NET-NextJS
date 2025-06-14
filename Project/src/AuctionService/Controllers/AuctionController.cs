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
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions(string? date)
    {
        DateTime parsedDate = default;
        var hasValidDate = !string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out parsedDate);
        var parsedUtcDate = hasValidDate ? parsedDate.ToUniversalTime() : DateTime.MinValue;

        var query =
            from auction in _context.Auctions.AsNoTracking()
            join item in _context.Items.AsNoTracking() on auction.Id equals item.AuctionId
            where !hasValidDate || auction.UpdatedAt > parsedUtcDate
            orderby item.Make
            select new AuctionDto
            {
                Id = auction.Id,
                CreatedAt = auction.CreatedAt,
                UpdatedAt = auction.UpdatedAt,
                AuctionEnd = auction.AuctionEnd,
                Seller = auction.Seller,
                Winner = auction.Winner,
                Status = auction.Status.ToString(),
                ReservePrice = auction.ReservePrice,
                SoldAmount = auction.SoldAmount,
                CurrentHighBid = auction.CurrentHighBid,
                Make = item.Make,
                Model = item.Model,
                Year = item.Year,
                Color = item.Color,
                Mileage = item.Mileage,
                ImageURL = item.ImageURL
            };

        var result = await query.ToListAsync();

        return result.Count > 0 ? Ok(result) : NotFound();
    }

    [Route("auction")]
    [HttpGet]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid auctionId)
    {
        var auction = await (from a in _context.Auctions
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

        var sellerUsername = User?.Identity?.Name ?? "unknown";

        var auction = new Auction
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

        var auctionDto = new AuctionDto
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

        var auction = new Auction { Id = result.AuctionId };
        _ = _context.Attach(auction);
        auction.UpdatedAt = DateTime.UtcNow;

        var item = new Item { Id = result.ItemId };
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
        var deletedCount = await _context.Auctions
            .Where(a => a.Id == auctionId)
            .ExecuteDeleteAsync();

        return deletedCount == 0 ? NotFound() : NoContent();
    }
}