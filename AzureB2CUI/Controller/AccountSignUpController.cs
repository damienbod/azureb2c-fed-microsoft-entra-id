using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Controllers
{
    [AllowAnonymous]
    [Route("MicrosoftIdentity/[controller]/[action]")]
    public class AccountSignUpController : Controller
    {
        private readonly IOptionsMonitor<MicrosoftIdentityOptions> _optionsMonitor;

        public AccountSignUpController(IOptionsMonitor<MicrosoftIdentityOptions> microsoftIdentityOptionsMonitor)
        {
            _optionsMonitor = microsoftIdentityOptionsMonitor;
        }

        [HttpGet("{scheme?}")]
        public IActionResult SignIn(
            [FromRoute] string scheme,
            [FromQuery] string redirectUri)
        {
            scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
            string redirect;
            if (!string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri))
            {
                redirect = redirectUri;
            }
            else
            {
                redirect = Url.Content("~/")!;
            }

            scheme ??= OpenIdConnectDefaults.AuthenticationScheme;

            var properties = new AuthenticationProperties { RedirectUri = redirect };
            properties.Items[Constants.Policy] = "B2C_1_signup";
            return Challenge(properties, scheme);
        }

    }
}