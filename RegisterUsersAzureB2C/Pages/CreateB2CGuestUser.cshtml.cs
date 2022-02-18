using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterUsersAzureB2C.CreateUser;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateB2CGuestUserModel : PageModel
{
    private readonly CreateUserService _createUserService;

    public CreateB2CGuestUserModel(CreateUserService createUserService)
    {
        _createUserService = createUserService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public UserModelB2CIdentity User { get; set; } = new UserModelB2CIdentity();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _createUserService.CreateGuestUserAsync(User);

        return RedirectToPage("./Index");
    }

}