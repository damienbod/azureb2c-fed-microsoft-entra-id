using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

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

        var users = await _graphServiceClient.Users.GetAsync((requestConfiguration) =>
        {
            requestConfiguration.QueryParameters.Filter = filter;
        });

        var userId = users!.Value!.FirstOrDefault()!.Id;

        if (string.IsNullOrEmpty(userId))
        {
            return string.Empty;
        }

        return userId;
    }

    public async Task SendEmailAsync(Message message)
    {
        var saveToSentItems = true;
        var userId = await GetUserIdAsync();

        var body = new SendMailPostRequestBody
        {
            Message = message,
            SaveToSentItems = saveToSentItems
        };

        await _graphServiceClient.Users[userId]
            .SendMail
            .PostAsync(body);
    }

}