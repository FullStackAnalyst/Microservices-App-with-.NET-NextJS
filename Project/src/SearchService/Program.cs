using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<AuctionSyncClient>()
    .AddPolicyHandler(ApplyRetryPolicy());

// Debug printouts to verify correct values
Console.WriteLine("RabbitMQ Host: " + builder.Configuration["RabbitMQ:HostName"]);
Console.WriteLine("RabbitMQ Username: " + builder.Configuration.GetValue("RabbitMQ:Username", "guest"));
Console.WriteLine("MongoDB Connection String: " + (builder.Configuration.GetConnectionString("SCS") ?? "NULL"));

builder.Services.AddMassTransit(o =>
{
    o.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    o.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    o.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ:HostName"];

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMQ:Username", "guest"));
            h.Password(builder.Configuration.GetValue("RabbitMQ:Password", "guest"));
        });

        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

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