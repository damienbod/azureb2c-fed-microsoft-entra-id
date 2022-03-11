using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterUsersAzureB2CMsGraph.CreateUser;
using RegisterUsersAzureB2CMsGraph.Services;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2CMsGraph.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateB2CFederatedUserModel : PageModel
{
    private readonly MsGraphService _msGraphService;

    public CreateB2CFederatedUserModel(MsGraphService msGraphService)
    {
        _msGraphService = msGraphService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public UserModelB2CIdentity UserModel { get; set; } = new UserModelB2CIdentity();

    [BindProperty]
    public string  Upn{ get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!_msGraphService.IsEmailValid(UserModel.Email))
        {
            ModelState.AddModelError("Email", "Email is invalid");
            return Page();
        }

        Upn = await _msGraphService.CreateFederatedNoPasswordAsync(UserModel);

        return OnGet();
    }
}