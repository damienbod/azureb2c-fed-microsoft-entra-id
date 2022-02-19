using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterUsersAzureB2C.CreateUser;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateAzureB2CUserModel : PageModel
{
    private readonly CreateUserService _createUserService;

    public CreateAzureB2CUserModel(CreateUserService createUserService)
    {
        _createUserService = createUserService;
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

        var data = await _createUserService.CreateAzureB2CUserAsync(UserModel);

        UserPassword = data.Password;
        return OnGet();
    }

}