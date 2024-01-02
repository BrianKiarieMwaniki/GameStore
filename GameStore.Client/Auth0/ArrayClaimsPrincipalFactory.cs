using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace GameStore.Client.Auth0;

public class ArrayClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    public ArrayClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor)
        : base(accessor)
    {
    }

    public async override ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);
        var claimsIdentity = (ClaimsIdentity?)user.Identity;

        if (account != null && claimsIdentity != null)
        {
            MapArrayClaimsToMultipleSeparateClaims(account, claimsIdentity);
        }

        return user;
    }

    private static void MapArrayClaimsToMultipleSeparateClaims(RemoteUserAccount account, ClaimsIdentity claimsIdentity)
    {
        foreach (var prop in account.AdditionalProperties)
        {
            var key = prop.Key;
            var value = prop.Value;
            if (value != null && value is JsonElement element && element.ValueKind == JsonValueKind.Array)
            {
                //Check if the claim exists before removing it
                var existingClaim = claimsIdentity.FindFirst(prop.Key);
                if(existingClaim != null)
                {
                    claimsIdentity.RemoveClaim(existingClaim);
                }

                if(key.Contains("role", StringComparison.OrdinalIgnoreCase))
                {
                    //Handle role claims explicitly
                    var roleClaims = element.EnumerateArray()
                    .Select(x => new Claim(ClaimTypes.Role, x.ToString()));
                    claimsIdentity.AddClaims(roleClaims);
                }
                else
                {
                    //Handle other array claims
                    var claims = element.EnumerateArray().Select(x => new Claim(prop.Key, x.ToString()));
                    claimsIdentity.AddClaims(claims);
                }               
            }
        }
    }
}