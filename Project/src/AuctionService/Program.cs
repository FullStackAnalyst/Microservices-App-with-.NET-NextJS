using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AuctionDataContext>(options => _ = options.UseNpgsql(builder.Configuration.GetConnectionString("")));

builder.Services.AddControllers();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();