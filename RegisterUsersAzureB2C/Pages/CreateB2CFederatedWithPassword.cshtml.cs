﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterUsersAzureB2C.CreateUser;
using RegisterUsersAzureB2C.Services;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Pages;

[Authorize(Policy = "IsAdminPolicy")]
public class CreateB2CFederatedWithPasswordUserModel : PageModel
{
    private readonly MsGraphService _msGraphService;

    public CreateB2CFederatedWithPasswordUserModel(MsGraphService msGraphService)
    {
        _msGraphService = msGraphService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public UserModelB2CIdentity UserModel { get; set; } = new UserModelB2CIdentity();

    [BindProperty]
    public string  UserPassword { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var (_, Password, _) = await _msGraphService.CreateFederatedUserWithPasswordAsync(UserModel);

        UserPassword = Password;
        return OnGet();
    }
}