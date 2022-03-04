using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using RegisterUsersAzureB2C.CreateUser;
using RegisterUsersAzureB2C.Services;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class InviteUserModel : PageModel
{
    private readonly MsGraphService _msGraphService;

    public InviteUserModel(MsGraphService msGraphService,
        IConfiguration configuration)
    {
        _msGraphService = msGraphService;
        AadB2CIssuerDomain = configuration.GetValue<string>("AzureAdB2C:Domain");
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public UserInvite UserInvite { get; set; } = new UserInvite();

    [BindProperty]
    public string AadB2CIssuerDomain { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!_msGraphService.IsEmailValid(UserInvite.Email))
        {
            ModelState.AddModelError("Email", "Email is invalid");
            return Page();
        }

        var invite = await _msGraphService.InviteUser(UserInvite.Email, "/profile");

        return OnGet();
    }

}