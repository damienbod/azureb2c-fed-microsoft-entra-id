using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using RegisterUsersAzureB2C.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RegisterUsersAzureB2C.Services;

public class MsGraphService
{
    private readonly GraphServiceClient _graphServiceClient;
    private string _aadIssuerDomain;
    private string _aadB2CIssuerDomain;

    public MsGraphService(IConfiguration configuration)
    {
        string[] scopes = configuration.GetValue<string>("GraphApi:Scopes")?.Split(' ');
        var tenantId = configuration.GetValue<string>("GraphApi:TenantId");

        // Values from app registration
        var clientId = configuration.GetValue<string>("GraphApi:ClientId");
        var clientSecret = configuration.GetValue<string>("GraphApi:ClientSecret");

        _aadIssuerDomain = configuration.GetValue<string>("AadIssuerDomain");
        _aadB2CIssuerDomain = configuration.GetValue<string>("AzureAdB2C:Domain");

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

        var test = await _graphServiceClient.Users[userId].Request().GetAsync();

        return await _graphServiceClient.Users[userId]
            .GetMemberGroups(securityEnabledOnly)
            .Request().PostAsync();
    }

    public async Task<(string Upn, string Password, string Id)> CreateAzureB2CSameDomainUserAsync(UserModelB2CTenant userModel)
    {
        if(!userModel.UserPrincipalName.ToLower().EndsWith(_aadB2CIssuerDomain.ToLower()))
        {
            throw new ArgumentException("incorrect Email domain");
        }
            
        var password = GetEncodedRandomString();
        var user = new User
        {
            AccountEnabled = true,
            UserPrincipalName = userModel.UserPrincipalName,
            DisplayName = userModel.DisplayName,
            Surname = userModel.Surname,
            GivenName = userModel.GivenName,
            PreferredLanguage = userModel.PreferredLanguage,
            MailNickname = userModel.DisplayName,
            // The following properties cannot be set in the initial POST request. Please set them in a subsequent PATCH request: birthday
            // Birthday = userModel.BirthDate,

            PasswordProfile = new PasswordProfile
            {
                ForceChangePasswordNextSignIn = true,
                Password = password
            }
        };

        await _graphServiceClient.Users
            .Request()
            .AddAsync(user);

        return (user.UserPrincipalName, user.PasswordProfile.Password, user.Id);
    }

    public async Task<(string Upn, string Password, string Id)> CreateFederatedUserWithPasswordAsync(UserModelB2CIdentity userModel)
    {
        // new user create, email does not matter unless you require to send mails
        var password = GetEncodedRandomString();
        var user = new User
        {
            DisplayName = userModel.DisplayName,
            PreferredLanguage = userModel.PreferredLanguage,
            Surname = userModel.Surname,
            GivenName = userModel.GivenName,
            OtherMails = new List<string> { userModel.Email },
            Identities = new List<ObjectIdentity>()
            {
                new ObjectIdentity
                {
                    SignInType = "federated",
                    Issuer = _aadB2CIssuerDomain,
                    IssuerAssignedId = userModel.Email
                },
            },
            PasswordProfile = new PasswordProfile
            {
                Password = password,
                ForceChangePasswordNextSignIn = false
            },
            PasswordPolicies = "DisablePasswordExpiration"
        };

        var createdUser = await _graphServiceClient.Users
            .Request()
            .AddAsync(user);

        return (createdUser.UserPrincipalName, user.PasswordProfile.Password, createdUser.Id);
    }

    public async Task<string> CreateFederatedNoPasswordAsync(UserModelB2CIdentity userModel)
    {
        // User must already exist in AAD
        var user = new User
        {
            DisplayName = userModel.DisplayName,
            PreferredLanguage = userModel.PreferredLanguage,
            Surname = userModel.Surname,
            GivenName = userModel.GivenName,
            OtherMails = new List<string> { userModel.Email },
            Identities = new List<ObjectIdentity>()
            {
                new ObjectIdentity
                {
                    SignInType = "federated",
                    Issuer = _aadIssuerDomain,
                    IssuerAssignedId = userModel.Email
                },
            }
        };

        var createdUser = await _graphServiceClient.Users
            .Request()
            .AddAsync(user);

        return createdUser.UserPrincipalName;
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

    private string GetEncodedRandomString()
    {
        var base64 = Convert.ToBase64String(GenerateRandomBytes(20));
        return HtmlEncoder.Default.Encode(base64);
    }

    private byte[] GenerateRandomBytes(int length)
    {
        var item = RandomNumberGenerator.Create();
        var byteArray = new byte[length];
        item.GetBytes(byteArray);
        return byteArray;
    }
}