using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DataSys.App.Tests.Test
{
    public class TestIdentityServerConfiguration
    {
        public static TestIdentityServerConfiguration Default = new TestIdentityServerConfiguration();

        private Client _testClient;
        public ApiResource Api { get; } = new ApiResource("api", "Api");

        public virtual Client TestClient => _testClient ?? (_testClient = MakeTestClient());

        public IEnumerable<ApiResource> ApiResources() => new[] {Api};

        protected virtual Client MakeTestClient() => new Client
        {
            ClientId = "TestClient",
            AllowedGrantTypes = {GrantType.ClientCredentials},
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = {Api.Name}
        };

        public IEnumerable<Client> Clients() => new[] {TestClient};

        public static ICollection<IdentityResource> IdentityResources() => new[] {new IdentityResources.OpenId()};

        public void SetupIdentityServices(IServiceCollection services)
        {
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(ApiResources())
                .AddInMemoryClients(Clients())
                .AddInMemoryIdentityResources(IdentityResources())
                .AddInMemoryPersistedGrants()
                ;
        }
    }
}