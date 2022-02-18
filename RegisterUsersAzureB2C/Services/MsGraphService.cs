using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using RegisterUsersAzureB2C.CreateUser;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Services;

public class MsGraphService
{
    private readonly GraphServiceClient _graphServiceClient;

    public MsGraphService(IConfiguration configuration)
    {
        string[] scopes = configuration.GetValue<string>("GraphApi:Scopes")?.Split(' ');
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

    public async Task CreateUserAsync(UserModel userModel)
    {
        var user = new User
        {
            DisplayName = "John Smith",
            Identities = new List<ObjectIdentity>()
            {
                new ObjectIdentity
                {
                    SignInType = "userName",
                    Issuer = "contoso.onmicrosoft.com",
                    IssuerAssignedId = "johnsmith"
                },
            },
            PasswordProfile = new PasswordProfile
            {
                Password = "password-value",
                ForceChangePasswordNextSignIn = false
            },
            PasswordPolicies = "DisablePasswordExpiration"
        };

        await _graphServiceClient.Users
            .Request()
            .AddAsync(user);

    }

    public async Task CreateFederatedUserAsync()
    {
        var user = new User
        {
            DisplayName = "John Smith",
            Identities = new List<ObjectIdentity>()
            {
                new ObjectIdentity
                {
                    SignInType = "federated",
                    Issuer = "facebook.com",
                    IssuerAssignedId = "5eecb0cd"
                },
            },
            PasswordProfile = new PasswordProfile
            {
                Password = "password-value",
                ForceChangePasswordNextSignIn = false
            },
            PasswordPolicies = "DisablePasswordExpiration"
        };

        await _graphServiceClient.Users
            .Request()
            .AddAsync(user);

    }
}