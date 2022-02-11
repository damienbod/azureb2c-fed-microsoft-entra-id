using System.Collections.Generic;
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
        string[] scopeRequiredByApi = new string[] { "access_as_user" };
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return new List<string> { "data for users and admins data" };
    }
}
