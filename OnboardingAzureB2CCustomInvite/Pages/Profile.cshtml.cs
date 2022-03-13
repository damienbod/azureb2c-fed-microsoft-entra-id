using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnboardingAzureB2CCustomInvite.Services;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingAzureB2CCustomInvite.Pages;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserService _userService;

    public ProfileModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public Profile Profile { get; set; } = new Profile();

    [BindProperty]
    public string? AzureOid { get; set; } = string.Empty;

    [BindProperty]
    public bool IsActive { get; set; }

    [BindProperty]
    public string? Email { get; set; }


    public async Task<IActionResult> OnGetAsync()
    {
        var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        var oid = User.Claims.FirstOrDefault(t => t.Type == oidClaimType)?.Value;
        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;

        UserEntity? userEntity = null;
        if (oid != null)
        {
            userEntity = await _userService.FindUserWithOid(oid);

            if (userEntity != null)
            {
                Profile.Surname = userEntity.Surname;
                Profile.FirstName = userEntity.FirstName;
                Profile.DisplayName = userEntity.DisplayName;
                Profile.BirthDate  = userEntity.BirthDate;

                IsActive = userEntity.IsActive;
                AzureOid = userEntity.AzureOid;
                Email = userEntity.Email;
            }
            else
            {
                IsActive = false;
                AzureOid = oid;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        var oid = User.Claims.FirstOrDefault(t => t.Type == oidClaimType)?.Value;
        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;

        UserEntity? userEntity = null;
        if (oid != null)
            userEntity = await _userService.FindUserWithOid(oid);
        
        if(userEntity == null)
        {
            userEntity = new UserEntity();

            if (oid != null)
                userEntity.AzureOid = oid;
            if (email != null)
                userEntity.Email = email;
        }

        userEntity.FirstName = Profile.FirstName;
        userEntity.Surname = Profile.Surname;
        userEntity.BirthDate = Profile.BirthDate;
        userEntity.DisplayName = Profile.DisplayName;
        userEntity.PreferredLanguage = Profile.PreferredLanguage;

        await _userService.UpdateCreateProfile(userEntity);

        return await OnGetAsync();
    }
}