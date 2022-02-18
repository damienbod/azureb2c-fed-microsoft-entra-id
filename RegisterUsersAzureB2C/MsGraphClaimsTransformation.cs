﻿using RegisterUsersAzureB2C.Services;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C;

public class MsGraphClaimsTransformation : IClaimsTransformation
{
    private readonly MsGraphService _msGraphService;

    public MsGraphClaimsTransformation(MsGraphService msGraphService)
    {

        _msGraphService = msGraphService;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        ClaimsIdentity claimsIdentity = new();
        var groupClaimType = "group";
        if (!principal.HasClaim(claim => claim.Type == groupClaimType))
        {
            var objectidentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            var objectIdentifier = principal.Claims.FirstOrDefault(t => t.Type == objectidentifierClaimType);

            var groupIds = await _msGraphService.GetGraphApiUserMemberGroups(objectIdentifier.Value);

            foreach (var groupId in groupIds.ToList())
            {
                claimsIdentity.AddClaim(new Claim(groupClaimType, groupId));
            }
        }

        principal.AddIdentity(claimsIdentity);
        return principal;
    }
}