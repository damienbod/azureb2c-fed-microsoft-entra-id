using AzureB2CUI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace AzureB2CUI.Pages
{
    [AuthorizeForScopes(Scopes = new string[] { "https://b2cdamienbod.onmicrosoft.com/723191f4-427e-4f77-93a8-0a62dac4e080/access_as_user" })]
    public class CallUserApiModel : PageModel
    {
        private readonly UserApiService _apiService;
        private readonly GraphApiClientService _graphApiClientService;

        public JArray DataFromApi { get; set; }
        public CallUserApiModel(UserApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            DataFromApi = await _apiService.GetApiDataAsync().ConfigureAwait(false);
        }
    }
}