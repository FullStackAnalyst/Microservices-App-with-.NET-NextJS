using AuctionService.Data;
using AuctionService.Entities.DTOs;
using AuctionService.Entities.Enums;
using AuctionService.Entities.Models;
using Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuctionController(AuctionDataContext context, IPublishEndpoint publish) : ControllerBase
{
    private readonly AuctionDataContext _context = context;
    private readonly IPublishEndpoint _publish = publish;

    [Route("auctions")]
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions([FromQuery] string? date)
    {
        var parsedDate = DateTime.MinValue;
        if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out var tempDate))
        {
            parsedDate = tempDate.ToUniversalTime();
        }

        var query =
            from auction in _context.Auctions.AsNoTracking()
            join item in _context.Items.AsNoTracking()
                on auction.Id equals item.AuctionId
            where auction.UpdatedAt > parsedDate
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
        var auction = await (
            from a in _context.Auctions
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

    [Authorize]
    [Route("add")]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction([FromBody] CreateAuctionDto dto)
    {
        var sellerName = User?.Identity?.Name;
        if (string.IsNullOrWhiteSpace(sellerName))
        {
            return BadRequest("Seller information is missing.");
        }

        var item = new Item
        {
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            Color = dto.Color,
            Mileage = dto.Mileage,
            ImageURL = dto.ImageUrl
        };

        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            Seller = sellerName.Trim(),
            ReservePrice = dto.ReservePrice,
            AuctionEnd = dto.AuctionEnd,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = Status.Live,
            Item = item
        };

        _ = await _context.Auctions.AddAsync(auction);

        var auctionDto = new AuctionDto
        {
            Id = auction.Id,
            CreatedAt = auction.CreatedAt,
            UpdatedAt = auction.UpdatedAt,
            AuctionEnd = auction.AuctionEnd,
            Seller = auction.Seller,
            Winner = auction.Winner,
            Make = item.Make,
            Model = item.Model,
            Year = item.Year,
            Color = item.Color,
            Mileage = item.Mileage,
            ImageURL = item.ImageURL,
            Status = auction.Status.ToString(),
            ReservePrice = auction.ReservePrice,
            SoldAmount = auction.SoldAmount,
            CurrentHighBid = auction.CurrentHighBid
        };

        await _publish.Publish(new AuctionCreated
        {
            Id = auctionDto.Id,
            Seller = auctionDto.Seller,
            Winner = auctionDto.Winner ?? string.Empty,
            Make = auctionDto.Make,
            Model = auctionDto.Model,
            Year = auctionDto.Year,
            Color = auctionDto.Color,
            Mileage = auctionDto.Mileage,
            ImageURL = auctionDto.ImageURL,
            ReservePrice = auctionDto.ReservePrice,
            AuctionEnd = auctionDto.AuctionEnd,
            CreatedAt = auctionDto.CreatedAt,
            UpdatedAt = auctionDto.UpdatedAt,
            Status = auctionDto.Status,
            SoldAmount = auctionDto.SoldAmount ?? 0,
            CurrentHighBid = auctionDto.CurrentHighBid ?? 0
        });

        var saveResult = await _context.SaveChangesAsync() > 0;

        return !saveResult
            ? BadRequest("Failed to create auction")
            : CreatedAtAction(nameof(GetAuction), new { auctionId = auction.Id }, auctionDto);
    }

    [Authorize]
    [Route("update")]
    [HttpPut]
    public async Task<IActionResult> PutAuction(Guid auctionId, [FromBody] UpdateAuctionDto dto)
    {
        var sellerName = User?.Identity?.Name;

        if (string.IsNullOrWhiteSpace(sellerName))
        {
            return Unauthorized("User identity is missing.");
        }

        var result = await (
            from a in _context.Auctions
            join i in _context.Items on a.Id equals i.AuctionId
            where a.Id == auctionId
            select new
            {
                AuctionId = a.Id,
                a.Seller,
                ItemId = i.Id,
                i.Make,
                i.Model,
                i.Color,
                i.Year,
                i.Mileage
            }
        ).FirstOrDefaultAsync();

        if (result is null)
        {
            return NotFound();
        }

        if (!string.Equals(result.Seller?.Trim(), sellerName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("You are not authorized to update this auction.");
        }

        var item = new Item { Id = result.ItemId };
        var entry = _context.Attach(item);
        var hasUpdate = false;

        if (!string.IsNullOrWhiteSpace(dto.Make) && dto.Make != result.Make)
        {
            item.Make = dto.Make;
            entry.Property(x => x.Make).IsModified = true;
            hasUpdate = true;
        }

        if (!string.IsNullOrWhiteSpace(dto.Model) && dto.Model != result.Model)
        {
            item.Model = dto.Model;
            entry.Property(x => x.Model).IsModified = true;
            hasUpdate = true;
        }

        if (!string.IsNullOrWhiteSpace(dto.Color) && dto.Color != result.Color)
        {
            item.Color = dto.Color;
            entry.Property(x => x.Color).IsModified = true;
            hasUpdate = true;
        }

        if (dto.Year.HasValue && dto.Year.Value != result.Year)
        {
            item.Year = dto.Year.Value;
            entry.Property(x => x.Year).IsModified = true;
            hasUpdate = true;
        }

        if (dto.Mileage.HasValue && dto.Mileage.Value != result.Mileage)
        {
            item.Mileage = dto.Mileage.Value;
            entry.Property(x => x.Mileage).IsModified = true;
            hasUpdate = true;
        }

        if (hasUpdate)
        {
            var auction = new Auction { Id = result.AuctionId };
            _context.Attach(auction).Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
            _context.Entry(auction).Property(x => x.UpdatedAt).IsModified = true;

            var auctionUpdated = new AuctionUpdated
            {
                Id = result.AuctionId.ToString(),
                Make = item.Make,
                Model = item.Model,
                Year = item.Year,
                Color = item.Color,
                Mileage = item.Mileage
            };

            await _publish.Publish(auctionUpdated);
        }

        var saved = await _context.SaveChangesAsync() > 0;
        return !saved ? BadRequest("Failed to update auction.") : NoContent();
    }

    [Authorize]
    [Route("delete")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAuction(Guid auctionId)
    {
        var auction = await _context.Auctions.FindAsync(auctionId);

        if (auction == null)
        {
            return NotFound();
        }

        var sellerName = User?.Identity?.Name;

        if (string.IsNullOrWhiteSpace(sellerName))
        {
            return Unauthorized("User identity is missing.");
        }

        if (!string.Equals(auction.Seller?.Trim(), sellerName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("You are not authorized to delete this auction.");
        }

        _ = _context.Auctions.Remove(auction);

        await _publish.Publish(new AuctionDeleted { Id = auction.Id.ToString() });

        var result = await _context.SaveChangesAsync() > 0;

        return !result ? BadRequest("Failed to delete auction") : NoContent();
    }
}