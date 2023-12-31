using Azure.Identity;
using Microsoft.Graph.Users.Item.GetMemberGroups;
using Microsoft.Graph.Models;
using Microsoft.Graph;

namespace AzureB2CUI.Services;

public class MsGraphService
{
    private readonly GraphServiceClient _graphServiceClient;

    public MsGraphService(IConfiguration configuration)
    {
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

    public async Task<User?> GetGraphApiUser(string userId)
    {
        return await _graphServiceClient.Users[userId]
                .GetAsync();
    }

    public async Task<AppRoleAssignmentCollectionResponse?> GetGraphApiUserAppRoles(string userId)
    {
        return await _graphServiceClient.Users[userId]
                .AppRoleAssignments
                .GetAsync();
    }

    public async Task<GetMemberGroupsPostResponse?> GetGraphApiUserMemberGroups(string userId)
    {
        var requestBody = new GetMemberGroupsPostRequestBody
        {
            SecurityEnabledOnly = true,
        };

        return await _graphServiceClient.Users[userId]
            .GetMemberGroups
            .PostAsGetMemberGroupsPostResponseAsync(requestBody);
    }
}