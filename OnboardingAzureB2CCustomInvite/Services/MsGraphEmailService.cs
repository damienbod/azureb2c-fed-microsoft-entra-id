using Azure.Identity;
using Microsoft.Graph;

namespace OnboardingAzureB2CCustomInvite.Services;

public class MsGraphEmailService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly IConfiguration _configuration;

    public MsGraphEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        string[]? scopes = configuration.GetValue<string>("AzureAdEmailService:Scopes")?.Split(' ');
        var tenantId = configuration.GetValue<string>("AzureAdEmailService:TenantId");

        // Values from app registration
        var clientId = configuration.GetValue<string>("AzureAdEmailService:ClientId");
        var clientSecret = configuration.GetValue<string>("AzureAdEmailService:ClientSecret");

        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };

        // https://docs.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
        var clientSecretCredential = new ClientSecretCredential(
            tenantId, clientId, clientSecret, options);

        _graphServiceClient = new GraphServiceClient(clientSecretCredential, scopes);
    }

    private async Task<string> GetUserIdAsync()
    {
        var meetingOrganizer = _configuration["AzureAdEmailService:EmailSender"];
        var filter = $"startswith(userPrincipalName,'{meetingOrganizer}')";

        var users = await _graphServiceClient.Users
            .Request()
            .Filter(filter)
            .GetAsync();

        return users.CurrentPage[0].Id;
    }

    public async Task SendEmailAsync(Message message)
    {
        var saveToSentItems = true;

        var userId = await GetUserIdAsync();

        await _graphServiceClient.Users[userId]
            .SendMail(message, saveToSentItems)
            .Request()
            .PostAsync();
    }
}