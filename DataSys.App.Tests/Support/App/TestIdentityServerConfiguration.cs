using System.Collections.Generic;
using DataSys.App.Tests.Support.Hosting;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DataSys.App.Tests.Support.App
{
    public class TestIdentityServerConfiguration : IIdentity4ServerConfiguration
    {
        public static TestIdentityServerConfiguration Default = new TestIdentityServerConfiguration();

        private Client _testClient;
        public ApiResource Api { get; } = new ApiResource("api", "Api");

        public virtual IEnumerable<Client> Id4Clients => new[] { _testClient ?? (_testClient = MakeTestClient()) };

        public string ClientSecret => "secret";

        public IEnumerable<ApiResource> ApiResources => new[] {Api};

        protected virtual Client MakeTestClient() => new Client
        {
            ClientId = "TestClient",
            AllowedGrantTypes = {GrantType.ClientCredentials},
            ClientSecrets =
            {
                new Secret(ClientSecret.Sha256())
            },
            AllowedScopes = {Api.Name}
        };

        public static ICollection<IdentityResource> IdentityResources => new[] {new IdentityResources.OpenId()};

        public void ConfigureServices(IServiceCollection svcs)
        {
            svcs.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(ApiResources)
                .AddInMemoryClients(Id4Clients)
                .AddInMemoryIdentityResources(IdentityResources)
                .AddInMemoryPersistedGrants()
                ;
        }
    }
}