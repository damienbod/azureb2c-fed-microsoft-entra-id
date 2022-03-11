using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using OnboardingAzureB2CCustomInvite.CreateUser;
using OnboardingAzureB2CCustomInvite.Services;
using System.Threading.Tasks;

namespace OnboardingAzureB2CCustomInvite.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateUserModel : PageModel
{
    private readonly MsGraphService _msGraphService;

    public CreateUserModel(MsGraphService msGraphService,
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
    public UserModelB2CTenant UserModel { get; set; } = new UserModelB2CTenant();

    [BindProperty]
    public string AadB2CIssuerDomain { get; set; }

    [BindProperty]
    public string UserPassword { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!_msGraphService.IsEmailValid(UserModel.UserPrincipalName))
        {
            ModelState.AddModelError("UserPrincipalName", "UserPrincipalName is invalid");
            return Page();
        }

        if (!UserModel.UserPrincipalName.ToLower().EndsWith(AadB2CIssuerDomain.ToLower()))
        {
            ModelState.AddModelError("UserPrincipalName", "UserPrincipalName domain is invalid");
            return Page();
        }

        return OnGet();
    }

}