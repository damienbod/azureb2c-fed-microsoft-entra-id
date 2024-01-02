using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AzureB2CUI;

public class UserApiService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public UserApiService(IHttpClientFactory clientFactory,
        ITokenAcquisition tokenAcquisition,
        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    public async Task<List<string>?> GetApiDataAsync()
    {
        var client = _clientFactory.CreateClient();

        var scope = _configuration["UserApiOne:ScopeForAccessToken"];
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope }!);

        client.BaseAddress = new Uri(_configuration["UserApiOne:ApiBaseAddress"]!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync("useraccess");
        if (response.IsSuccessStatusCode)
        {
            var data = await JsonSerializer.DeserializeAsync<List<string>>(
                await response.Content.ReadAsStreamAsync());

            return data;
        }

        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
    }
}