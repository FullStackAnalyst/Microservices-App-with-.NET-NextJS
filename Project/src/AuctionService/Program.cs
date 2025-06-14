using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AuctionDataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DCS")));

builder.Services.AddControllers();

builder.Services.AddMassTransit(o =>
{
    o.AddEntityFrameworkOutbox<AuctionDataContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        _ = o.UsePostgres();

        o.UseBusOutbox();
    });

    o.UsingRabbitMq((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();