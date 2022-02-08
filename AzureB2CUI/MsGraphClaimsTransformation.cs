using AzureB2CUI.Services;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AzureB2CUI
{
    public class MsGraphClaimsTransformation : IClaimsTransformation
    {
        private MsGraphService _msGraphService;

        public MsGraphClaimsTransformation(MsGraphService msGraphService)
        {

            _msGraphService = msGraphService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var groupClaimType = "group";
            if (!principal.HasClaim(claim => claim.Type == groupClaimType))
            {
                var nameidentifierClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
                var nameidentifier = principal.Claims.FirstOrDefault(t => t.Type == nameidentifierClaimType);

                var groupIds = await _msGraphService.GetGraphApiUserMemberGroups(nameidentifier.Value);

                foreach (var groupId in groupIds.ToList())
                {
                    claimsIdentity.AddClaim(new Claim(groupClaimType, groupId));
                }
            }

            principal.AddIdentity(claimsIdentity);
            return principal;
        }


    }
}
