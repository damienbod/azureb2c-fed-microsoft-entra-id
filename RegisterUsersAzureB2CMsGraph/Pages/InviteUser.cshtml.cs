﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterUsersAzureB2CMsGraph.CreateUser;
using RegisterUsersAzureB2CMsGraph.Services;

namespace RegisterUsersAzureB2CMsGraph.Pages;

/// <summary>
/// Graph invitations only works for Azure AD, not Azure B2C
/// </summary>
[Authorize(Policy = "IsAdminPolicy")]
public class InviteUserModel : PageModel
{
    private readonly MsGraphService _msGraphService;

    public InviteUserModel(MsGraphService msGraphService,
        IConfiguration configuration)
    {
        _msGraphService = msGraphService;
        AadB2CIssuerDomain = configuration.GetValue<string>("AzureAdB2C:Domain")!;
    }

    [BindProperty]
    public UserInvite UserInvite { get; set; } = new UserInvite();

    [BindProperty]
    public string? InviteRedeemUrl { get; set; }

    [BindProperty]
    public string AadB2CIssuerDomain { get; set; }

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

        if (!_msGraphService.IsEmailValid(UserInvite.Email))
        {
            ModelState.AddModelError("Email", "Email is invalid");
            return Page();
        }

        var invite = await _msGraphService.InviteUser(UserInvite.Email,
            "https://localhost:44397/profile");

        InviteRedeemUrl = invite.InviteRedeemUrl;

        return OnGet();
    }

}