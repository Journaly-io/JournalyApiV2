﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace JournalyApiV2.Pipeline;


public class ClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = principal.Identity as ClaimsIdentity;

        if (claimsIdentity != null)
        {
            if (!claimsIdentity.HasClaim(c => c.Type == "token_id"))
            {
                var tokenIdClaim = claimsIdentity.FindFirst("your_token_id_claim_name_in_token");

                if (tokenIdClaim != null)
                {
                    claimsIdentity.AddClaim(new Claim("token_id", tokenIdClaim.Value));
                }
            }
        }

        return Task.FromResult(principal);
    }
}