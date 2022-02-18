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
    public UserModelB2CTenant User { get; set; } = new UserModelB2CTenant();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _createUserService.CreateAzureB2CUserAsync(User);

        return RedirectToPage("./Index");
    }

}