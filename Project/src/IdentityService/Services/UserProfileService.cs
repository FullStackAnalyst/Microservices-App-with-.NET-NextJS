using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityService.Services;

public class UserProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await userManager.GetUserAsync(context.Subject)
                   ?? throw new Exception("User not found");

        var userClaims = await userManager.GetClaimsAsync(user);

        var claims = new List<Claim>
        {
            new("username", user.UserName ?? string.Empty),
        };

        context.IssuedClaims.AddRange(claims);

        var nameClaim = userClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name);

        if (nameClaim != null)
        {
            context.IssuedClaims.Add(nameClaim);
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}