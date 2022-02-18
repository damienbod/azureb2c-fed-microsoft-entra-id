using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterUsersAzureB2C.CreateUser;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateInternalUserModel : PageModel
{
    private readonly CreateUserService _createUserService;

    public CreateInternalUserModel(CreateUserService createUserService)
    {
        _createUserService = createUserService;
    }

    public IActionResult OnGet()
    {
        User = new UserModel(); 
        return Page();
    }

    [BindProperty]
    public UserModel User { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _createUserService.CreateUserAsync(User);

        return RedirectToPage("./Index");
    }

}