﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnboardingAzureB2CCustomInvite.Services;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingAzureB2CCustomInvite.Pages;

public class ConnectAccountModel : PageModel
{
    private readonly UserService _userService;

    public ConnectAccountModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public string? OnboardingRegistrationCode { get; set; } = string.Empty;

    public async Task<IActionResult> OnGet()
    {
        if (string.IsNullOrEmpty(OnboardingRegistrationCode))
        {
            return Page();
        }

        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        var oid = User.Claims.FirstOrDefault(t => t.Type == oidClaimType)?.Value;

        if(oid == null)
            return Page();

        int id = await _userService.UpdateUserIfExistsAsync(
            OnboardingRegistrationCode, oid, email);

        if(id > 0)
        {
            return Redirect("/profile");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        var oid = User.Claims.FirstOrDefault(t => t.Type == oidClaimType)?.Value;

        int id = await _userService.UpdateUserIfExistsAsync(
            OnboardingRegistrationCode, oid, email);

        if (id > 0)
        {
            return Redirect("/profile");
        }

        return Page();
    }
}
