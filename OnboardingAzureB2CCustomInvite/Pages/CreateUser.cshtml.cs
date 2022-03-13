using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnboardingAzureB2CCustomInvite.Services;
using System.Threading.Tasks;

namespace OnboardingAzureB2CCustomInvite.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateUserModel : PageModel
{
    private readonly MsGraphEmailService _msGraphEmailService;
    private readonly UserService _userService;
    private readonly EmailService _emailService;

    public CreateUserModel(MsGraphEmailService msGraphEmailService,
        UserService userService, EmailService emailService)
    {
        _msGraphEmailService = msGraphEmailService;
        _userService = userService;
        _emailService = emailService;
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

        await _userService.SendEmailInvite(user, Request.Host);

        OnboardingRegistrationCode = user.OnboardingRegistrationCode;
        return OnGet();
    }
}