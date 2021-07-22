using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AzureB2CUI.Pages
{
    public class CallAdminApiModel : PageModel
    {
        private readonly AdminApiOneService _apiService;

        public JArray DataFromApi { get; set; }
        public CallAdminApiModel(AdminApiOneService apiService)
        {
            _apiService = apiService;
        }

        public async Task OnGetAsync()
        {
            DataFromApi = await _apiService.GetApiDataAsync().ConfigureAwait(false);
        }
    }
}