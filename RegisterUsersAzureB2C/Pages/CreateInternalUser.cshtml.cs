using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public async Task OnGetAsync()
    {
        
    }

    public async Task OnPostAsync()
    {
        await _createUserService.CreateUserAsync();
    }
}