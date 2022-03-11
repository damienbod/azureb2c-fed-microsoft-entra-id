using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using OnboardingAzureB2CCustomInvite.CreateUser;
using OnboardingAzureB2CCustomInvite.Services;
using System.Threading.Tasks;

namespace OnboardingAzureB2CCustomInvite.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateUserModel : PageModel
{
    private readonly MsGraphService _msGraphService;
    private readonly UserService _userService;

    public CreateUserModel(MsGraphService msGraphService,
        UserService userService, IConfiguration configuration)
    {
        _msGraphService = msGraphService;
        _userService = userService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public UserModel UserModel { get; set; } = new UserModel();

    [BindProperty]
    public string OnboardingRegistrationCode { get; set; } = string.Empty;

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

        return OnGet();
    }

}