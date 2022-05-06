using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AzureB2CUI.Pages
{
    [AllowAnonymous]
    public class SignUpModel : PageModel
    {
        public async Task OnGetAsync()
        {
            await HttpContext.ChallengeAsync("signuppolicy");
        }
    }
}
