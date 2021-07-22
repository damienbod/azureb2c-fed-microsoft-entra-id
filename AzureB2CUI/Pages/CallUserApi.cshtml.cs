using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AzureB2CUI.Pages
{
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