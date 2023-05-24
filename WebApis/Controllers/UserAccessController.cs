using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace WebApis.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[AuthorizeForScopes(Scopes = new string[] { "api://723191f4-427e-4f77-93a8-0a62dac4e080/access_as_user" })]
[ApiController]
[Route("[controller]")]
public class UserAccessController : ControllerBase
{
    [HttpGet]
    public List<string> Get()
    {
        string[] scopeRequiredByApi = new string[] { "access_as_user" };
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return new List<string> { "user data" };
    }
}
