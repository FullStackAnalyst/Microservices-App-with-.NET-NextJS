using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = builder.Configuration["IdentityServiceURL"];
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters.ValidateAudience = false;
        o.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

app.MapReverseProxy();

app.UseAuthentication();

app.UseAuthorization();

app.Run();