using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new("auctionApp", "Auction app full access.")
        ];

    public static IEnumerable<Client> Clients =>
        [
            new() {
                ClientId = "postman",
                ClientName = "Postman",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                ClientSecrets = { new Secret("NotASecret".Sha256()) },

                AllowedScopes = { "openid", "profile", "auctionApp" },
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600,
            },

            new() {
                ClientId = "nextApp",
                ClientName = "Next.js Frontend",

                AllowedGrantTypes = GrantTypes.Code,

                RequireClientSecret = false,
                RequirePkce = true,
                RedirectUris = { "https://localhost:7575/api/auth/callback/id-server" },
                PostLogoutRedirectUris = { "https://localhost:7575" },

                AllowedScopes = { "openid", "profile", "auctionApp" },
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600 * 24 * 30,
                AlwaysIncludeUserClaimsInIdToken = true
            }
        ];
}