using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace WebApis.Controllers;

[Authorize(AuthenticationSchemes = "Bearer, BearerAdmin")]
[ApiController]
[Route("[controller]")]
public class UserOrAdminAccessController : ControllerBase
{
    [HttpGet]
    public List<string> Get()
    {
        // could be from either API token
        string[] scopeRequiredByApi = ["access_as_user"];
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return ["data for users and admins data"];
    }
}
