using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace GameStore.Api.Authorization;

public class ScopeTransformation : IClaimsTransformation
{
    private const string scopeClaimName = "scope";
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var scopeClaim = principal.FindFirst(scopeClaimName);

        if (scopeClaim is null) return Task.FromResult(principal);

        var scopes = scopeClaim.Value.Split(' ');


        var originalIdentity = principal.Identity as ClaimsIdentity;


        var identity = new ClaimsIdentity(originalIdentity);

        var originalScopeClaim = identity.Claims.FirstOrDefault(claim => claim.Type == scopeClaimName);
        if (originalScopeClaim is not null)
        {
            identity.RemoveClaim(originalScopeClaim);
        }

        var roleClaims = originalIdentity?.Claims
                    .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/roles")
                    .ToList();

        identity.AddClaims(scopes.Select(scope => new Claim(scopeClaimName, scope)));

        if (roleClaims != null && roleClaims.Any())
        {
            foreach (var claim in roleClaims)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, claim.Value));
            }
        }


        return Task.FromResult(new ClaimsPrincipal(identity));
    }
}