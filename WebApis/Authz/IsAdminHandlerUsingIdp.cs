using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApis.Authz
{
    public class IsAdminHandlerUsingIdp : AuthorizationHandler<IsAdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (requirement == null)
                throw new ArgumentNullException(nameof(requirement));

            var claimIdentityprovider = context.User.Claims.FirstOrDefault(t => t.Type == "idp");

            // check that our tenant was used to signin
            if (claimIdentityprovider != null
                && claimIdentityprovider.Value == "https://login.microsoftonline.com/7ff95b15-dc21-4ba6-bc92-824856578fc1/v2.0")
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}