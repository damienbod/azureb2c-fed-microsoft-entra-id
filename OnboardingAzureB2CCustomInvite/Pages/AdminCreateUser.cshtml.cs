using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnboardingAzureB2CCustomInvite.Services;

namespace OnboardingAzureB2CCustomInvite.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class AdminCreateUserModel : PageModel
{
    private readonly UserService _userService;

    public AdminCreateUserModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public UserModel UserModel { get; set; } = new UserModel();

    [BindProperty]
    public string? OnboardingRegistrationCode { get; set; } = string.Empty;

    [BindProperty]
    public string? AccountUrl { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!_userService.IsEmailValid(UserModel.Email))
        {
            ModelState.AddModelError("Email", "Email is invalid");
            return Page();
        }

        var user = await _userService.CreateUser(new UserEntity
        {
            Email = UserModel.Email,
            FirstName = UserModel.FirstName,
            Surname = UserModel.Surname,
            BirthDate = UserModel.BirthDate,
            DisplayName = UserModel.DisplayName,
            PreferredLanguage = UserModel.PreferredLanguage
        });

        await _userService.SendEmailInvite(user, Request.Host, false);

        OnboardingRegistrationCode = user.OnboardingRegistrationCode;
        return OnGet();
    }
}