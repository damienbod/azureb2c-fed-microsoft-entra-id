using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using RegisterUsersAzureB2CMsGraph.CreateUser;
using RegisterUsersAzureB2CMsGraph.Services;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2CMsGraph.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateAzureB2CUserModel : PageModel
{
    private readonly MsGraphService _msGraphService;

    public CreateAzureB2CUserModel(MsGraphService msGraphService,
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

        var (_, Password, _) = await _msGraphService.CreateAzureB2CSameDomainUserAsync(UserModel);

        UserPassword = Password;
        return OnGet();
    }

}