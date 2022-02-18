using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateUserApiModel : PageModel
{
    private readonly CreateUserService _createUserService;

    public CreateUserApiModel(CreateUserService createUserService)
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