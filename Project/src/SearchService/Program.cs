using SearchService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    await Initializer.InitializeDB(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

await app.RunAsync();
