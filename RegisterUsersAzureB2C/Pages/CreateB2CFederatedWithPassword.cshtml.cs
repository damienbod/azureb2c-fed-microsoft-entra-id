using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterUsersAzureB2C.CreateUser;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateB2CFederatedWithPasswordUserModel : PageModel
{
    private readonly CreateUserService _createUserService;

    public CreateB2CFederatedWithPasswordUserModel(CreateUserService createUserService)
    {
        _createUserService = createUserService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public UserModelB2CIdentity UserModel { get; set; } = new UserModelB2CIdentity();

    [BindProperty]
    public string  UserPassword { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        //var data = await _createUserService.CreateFederatedUserAsync(UserModel);
        var data = await _createUserService.CreateFederatedToMyAADAsync(UserModel);

        UserPassword = data;
        return OnGet();
    }
}