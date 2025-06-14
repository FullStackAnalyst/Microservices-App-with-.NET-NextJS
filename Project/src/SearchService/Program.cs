using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpClient<AuctionSyncClient>()
    .AddPolicyHandler(ApplyRetryPolicy());

builder.Services.AddMassTransit(o =>
{
    o.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    o.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    o.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", _ => { });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await Initializer.ConfigureDatabase(app);

await app.RunAsync();

static IAsyncPolicy<HttpResponseMessage> ApplyRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
}