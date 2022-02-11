using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AzureB2CUI.Pages;

[Authorize(Policy = "IsAdminPolicy")]
[AuthorizeForScopes(Scopes = new string[] { "https://b2cdamienbod.onmicrosoft.com/5f4e8bb1-3f4e-4fc6-b03c-12169e192cd7/access_as_user" })]
public class CallAdminApiModel : PageModel
{
    private readonly AdminApiService _apiService;

    public JArray DataFromApi { get; set; }
    public CallAdminApiModel(AdminApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        DataFromApi = await _apiService.GetApiDataAsync();
    }
}