using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System.Threading.Tasks;
using System;

namespace AzureB2CUI.Services
{
    public class GraphApiClientService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public GraphApiClientService(IConfiguration configuration)
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
            try
            {
                var d2 = await _graphServiceClient.Users[userId]
                    .Request()
                    .GetAsync()
                    .ConfigureAwait(false);

                return d2;

            }
            catch (Exception ex)
            {
                var da =  ex.Message;
                return null;
            }
            
        }
    }
}

