using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace AzureB2CUI.Pages;

[AuthorizeForScopes(Scopes = ["https://b2cdamienbod.onmicrosoft.com/723191f4-427e-4f77-93a8-0a62dac4e080/access_as_user"])]
public class CallUserApiModel : PageModel
{
    private readonly UserApiService _userApiService;

    public List<string>? DataFromApi { get; set; }

    public CallUserApiModel(UserApiService userApiService)
    {
        _userApiService = userApiService;
    }

    public async Task OnGetAsync()
    {
        DataFromApi = await _userApiService.GetApiDataAsync();
    }
}