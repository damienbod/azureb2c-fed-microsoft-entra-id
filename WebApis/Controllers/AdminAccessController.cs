using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace WebApis.Controllers
{
    [Authorize]
    [AuthorizeForScopes(Scopes = new string[] { "api://72286b8d-5010-4632-9cea-e69e565a5517/access_as_user" })]
    [ApiController]
    [Route("[controller]")]
    public class AdminAccessController : ControllerBase
    {
        [HttpGet]
        public List<string> Get()
        {
            string[] scopeRequiredByApi = new string[] { "access_as_user" };
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            return new List<string> { "user data" };
        }
    }
}
