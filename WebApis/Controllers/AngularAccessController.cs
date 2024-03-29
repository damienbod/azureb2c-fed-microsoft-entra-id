﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace WebApis.Controllers;

[Authorize(AuthenticationSchemes = "BearerAngularApi")]
[AuthorizeForScopes(Scopes = ["api://ac9b845d-96d3-4410-9923-50ec7bc80db9/access_as_user"])]
[ApiController]
[Route("[controller]")]
public class AngularAccessController : ControllerBase
{
    [HttpGet]
    public List<string> Get()
    {
        string[] scopeRequiredByApi = ["access_as_user"];
        HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

        return ["data for Angular public client PKCE"];
    }
}
