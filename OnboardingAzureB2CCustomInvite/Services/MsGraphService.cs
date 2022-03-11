using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OnboardingAzureB2CCustomInvite.Services;

public class MsGraphService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly IConfiguration _configuration;

    public MsGraphService(IConfiguration configuration)
    {
        _configuration = configuration;
        string[]? scopes = configuration.GetValue<string>("GraphApi:Scopes")?.Split(' ');
        var tenantId = configuration.GetValue<string>("GraphApi:TenantId");

        // Values from app registration
        var clientId = configuration.GetValue<string>("GraphApi:ClientId");
        var clientSecret = configuration.GetValue<string>("GraphApi:ClientSecret");

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
        var meetingOrganizer = _configuration["AzureAd:EmailSender"];
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

    public async Task<User> GetGraphApiUser(string userId)
    {
        return await _graphServiceClient.Users[userId]
            .Request()
            .GetAsync();
    }

    public async Task<IUserAppRoleAssignmentsCollectionPage> GetGraphApiUserAppRoles(string userId)
    {
        return await _graphServiceClient.Users[userId]
            .AppRoleAssignments
            .Request()
            .GetAsync();
    }

    public async Task<IDirectoryObjectGetMemberGroupsCollectionPage> GetGraphApiUserMemberGroups(string userId)
    {
        var securityEnabledOnly = true;

        return await _graphServiceClient.Users[userId]
            .GetMemberGroups(securityEnabledOnly)
            .Request().PostAsync();
    }

    public bool IsEmailValid(string email)
    {
        if (!MailAddress.TryCreate(email, out var mailAddress))
            return false;

        // And if you want to be more strict:
        var hostParts = mailAddress.Host.Split('.');
        if (hostParts.Length == 1)
            return false; // No dot.
        if (hostParts.Any(p => p == string.Empty))
            return false; // Double dot.
        if (hostParts[^1].Length < 2)
            return false; // TLD only one letter.

        if (mailAddress.User.Contains(' '))
            return false;
        if (mailAddress.User.Split('.').Any(p => p == string.Empty))
            return false; // Double dot or dot at end of user part.

        return true;
    }
}