﻿using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using RegisterUsersAzureB2C.CreateUser;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
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

        var test = await _graphServiceClient.Users[userId].Request().GetAsync();

        return await _graphServiceClient.Users[userId]
            .GetMemberGroups(securityEnabledOnly)
            .Request().PostAsync();
    }

    public async Task CreateAzureB2CUserAsync(UserModel userModel)
    {
        var user = new User
        {
            AccountEnabled = true,
            DisplayName = "Nick Moore",
            MailNickname = "NickMoore",
            PreferredLanguage = "de",
            GivenName = "nick",
            Surname= "Moore",
            Birthday = DateTime.Now.AddYears(-25),
            UserPrincipalName = "nick.moore@b2cdamienbod.onmicrosoft.com",
            PasswordProfile = new PasswordProfile
            {
                ForceChangePasswordNextSignIn = true,
                Password = "xWwvJ]6NMw+bWH-d"
            }
        };

        await _graphServiceClient.Users
            .Request()
            .AddAsync(user);
    }

    public async Task CreateFederatedUserAsync(UserModel userModel)
    {
        var user = new User
        {
            DisplayName = "John Smith",
            UserPrincipalName = "AdeleV@contoso.onmicrosoft.com",
            Identities = new List<ObjectIdentity>()
            {
                new ObjectIdentity
                {
                    SignInType = "federated",
                    Issuer = "damienbodhotmail.onmicrosoft.com",
                    IssuerAssignedId = GetEncodedRandomString()
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