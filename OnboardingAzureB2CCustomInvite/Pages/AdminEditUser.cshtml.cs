using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnboardingAzureB2CCustomInvite.Services;
using System.Threading.Tasks;

namespace OnboardingAzureB2CCustomInvite.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class AdminEditUserModel : PageModel
{
    private readonly UserService _userService;

    public AdminEditUserModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty]
    public UserModel UserModel { get; set; } = new UserModel();

    public async Task<IActionResult> OnGet()
    {
        var userEntity = await _userService.GetUserById(Id);

        UserModel.Surname = userEntity.Surname;
        UserModel.FirstName = userEntity.FirstName;
        UserModel.DisplayName = userEntity.DisplayName;
        UserModel.BirthDate = userEntity.BirthDate;

        UserModel.IsActive = userEntity.IsActive;
        UserModel.AzureOid = userEntity.AzureOid;
        UserModel.Email = userEntity.Email;

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

        await _userService.UpdateUser(UserModel, Id);

        return Page();
    }

    public async Task<IActionResult> SendInviteAsync()
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

        var userEntity = await _userService.GetUserById(Id);
        await _userService.SendEmailInvite(userEntity, Request.Host, true);

        return Page();
    }


}