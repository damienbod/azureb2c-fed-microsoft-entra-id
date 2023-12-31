using Microsoft.AspNetCore.Authorization;

namespace OnboardingAzureB2CCustomInvite.Authz;

public class IsAdminHandlerUsingAzureGroups : AuthorizationHandler<IsAdminRequirement>
{
    private readonly string _adminGroupId;

    public IsAdminHandlerUsingAzureGroups(IConfiguration configuration)
    {
        _adminGroupId = configuration.GetValue<string>("AzureGroups:AdminGroupId")!;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        var claimIdentityprovider = context.User.Claims.FirstOrDefault(t => t.Type == "group"
            && t.Value == _adminGroupId);

        if (claimIdentityprovider != null)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}