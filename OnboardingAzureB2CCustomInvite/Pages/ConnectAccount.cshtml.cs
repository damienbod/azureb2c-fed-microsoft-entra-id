using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnboardingAzureB2CCustomInvite.Pages;

public class ConnectAccountModel : PageModel
{
    [BindProperty]
    public string OnboardingRegistrationCode { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public void OnPost()
    {
    }
}
