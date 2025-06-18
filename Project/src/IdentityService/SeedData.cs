using Duende.IdentityModel;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace IdentityService;
public static class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (!userMgr.Users.Any())
        {
            CreateUser(userMgr, "alice", "AliceSmith@example.com");
            CreateUser(userMgr, "bob", "BobSmith@example.com");
        }
    }

    private static void CreateUser(UserManager<ApplicationUser> userMgr, string username, string email)
    {
        var user = userMgr.FindByNameAsync(username).Result;

        if (user != null)
        {
            Log.Debug($"{username} already exists");
            return;
        }

        user = new ApplicationUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true
        };

        var result = userMgr.CreateAsync(user, "Pass123$").Result;

        if (!result.Succeeded)
        {
            throw new Exception($"{username} creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        result = userMgr.AddClaimsAsync(user, [
            new Claim(JwtClaimTypes.Name, $"{char.ToUpper(username[0]) + username[1..]} Smith")
        ]).Result;

        if (!result.Succeeded)
        {
            throw new Exception($"{username} claim assignment failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        Log.Debug($"{username} created");
    }
}