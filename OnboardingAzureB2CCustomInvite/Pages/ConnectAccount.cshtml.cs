using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnboardingAzureB2CCustomInvite.Services;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingAzureB2CCustomInvite.Pages;

[Authorize]
public class ConnectAccountModel : PageModel
{
    private readonly UserService _userService;

    public ConnectAccountModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public string? OnboardingRegistrationCode { get; set; } = string.Empty;

    public async Task<IActionResult> OnGet([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return Page();
        }

        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        var oid = User.Claims.FirstOrDefault(t => t.Type == oidClaimType)?.Value;

        if(oid == null)
            return Page();

        int id = await _userService.UpdateUserIfExistsAsync(
            code, oid, email);

        if(id > 0)
        {
            return Redirect("/profile");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if(string.IsNullOrEmpty(OnboardingRegistrationCode))
        {
            ModelState.AddModelError("OnboardingRegistrationCode", "code required");
            return Page();
        }

        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        var oid = User.Claims.FirstOrDefault(t => t.Type == oidClaimType)?.Value;

        int id = await _userService.UpdateUserIfExistsAsync(
            OnboardingRegistrationCode, oid, email);

        if (id > 0)
        {
            return Redirect("/profile");
        }

        return Page();
    }
}
