using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterUsersAzureB2C.CreateUser;
using RegisterUsersAzureB2C.Services;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateAzureB2CUserModel : PageModel
{
    private readonly MsGraphService _msGraphService;

    public CreateAzureB2CUserModel(MsGraphService msGraphService)
    {
        _msGraphService = msGraphService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public UserModelB2CTenant UserModel { get; set; } = new UserModelB2CTenant();

    [BindProperty]
    public string UserPassword { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var (_, Password, _) = await _msGraphService.CreateAzureB2CSameDomainUserAsync(UserModel);

        UserPassword = Password;
        return OnGet();
    }

}