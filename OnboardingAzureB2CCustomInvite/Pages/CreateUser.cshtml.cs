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
    private readonly EmailService _emailService;

    public CreateUserModel(MsGraphService msGraphService,
        UserService userService,
        EmailService emailService,
        IConfiguration configuration)
    {
        _msGraphService = msGraphService;
        _userService = userService;
        _emailService = emailService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public UserModel UserModel { get; set; } = new UserModel();

    [BindProperty]
    public string OnboardingRegistrationCode { get; set; } = string.Empty;

    [BindProperty]
    public string AccountUrl { get; set; } = string.Empty;

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

        var user = await _userService.CreateUser(new UserEntity
        {
            Email = UserModel.Email,
            FirstName = UserModel.FirstName,
            Surname = UserModel.Surname,
            BirthDate = UserModel.BirthDate,
            DisplayName = UserModel.DisplayName,
            PreferredLanguage = UserModel.PreferredLanguage
        });

        AccountUrl = $"{Request.Host}/ConnectAccount/{user.OnboardingRegistrationCode}";
        var header = $"{user.FirstName} {user.Surname} you are invited to signup";
        var body = $"Hi {user.FirstName} {user.Surname} \n Use the following link to register \n {AccountUrl}";
        var message = _emailService.CreateStandardEmail(user.Email, header, body);

        await _msGraphService.SendEmailAsync(message);

        OnboardingRegistrationCode = user.OnboardingRegistrationCode;
        return OnGet();
    }

}