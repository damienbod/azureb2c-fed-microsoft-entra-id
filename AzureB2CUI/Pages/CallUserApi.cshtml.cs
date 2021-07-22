using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AzureB2CUI.Pages
{
    [AuthorizeForScopes(Scopes = new string[] { "https://b2cdamienbod.onmicrosoft.com/723191f4-427e-4f77-93a8-0a62dac4e080/access_as_user" })]
    public class CallUserApiModel : PageModel
    {
        private readonly UserApiOneService _apiService;

        public JArray DataFromApi { get; set; }
        public CallUserApiModel(UserApiOneService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            DataFromApi = await _apiService.GetApiDataAsync().ConfigureAwait(false);
        }
    }
}