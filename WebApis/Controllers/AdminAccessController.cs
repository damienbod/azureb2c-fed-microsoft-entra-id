using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace WebApis.Controllers;

[Authorize(AuthenticationSchemes = "BearerAdmin", Policy = "IsAdminRequirementPolicy")]
[AuthorizeForScopes(Scopes = new string[] { "api://5f4e8bb1-3f4e-4fc6-b03c-12169e192cd7/access_as_user" })]
[ApiController]
[Route("[controller]")]
public class AdminAccessController : ControllerBase
{
    [HttpGet]
    public List<string> Get()
    {
        string[] scopeRequiredByApi = new string[] { "access_as_user" };
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return new List<string> { "admin data" };
    }
}
