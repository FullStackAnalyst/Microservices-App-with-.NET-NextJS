using AuctionService.Consumers;
using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

    o.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

    o.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

    o.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = builder.Configuration["IdentityServiceURL"];
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters.ValidateAudience = false;
        o.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();