using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataSys.App.DataAccess;
using DataSys.App.Hosting;
using DataSys.App.Tests.Support;
using IdentityModel.Client;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace DataSys.App.Tests.Test
{
    public class TestSourceBasedTests : TestSourceBasedTests<TestSourceBasedTests.TestStartup>
    {
        public class TestStartup : Startup
        {
        }

        public TestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }
    }
    public class SecurityTestSourceBasedTests: TestSourceBasedTests<SecurityTestSourceBasedTests.TestStartup>
    {
        public SecurityTestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        public class TestStartup : TestSourceBasedTests.TestStartup
        {
            public virtual TestIdentityServerConfiguration TestIdentityServerConfiguration() => Test.TestIdentityServerConfiguration.Default;
            public override void ConfigureServices(IServiceCollection services)
            {
                SetupTrustedAuthentication(services, "http://localhost/");
                TestIdentityServerConfiguration().SetupIdentityServices(services);
                base.ConfigureServices(services);
            }
            public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                base.Configure(app, env);
                app.UseIdentityServer();
            }
            public void SetupTrustedAuthentication(IServiceCollection svcs, string baseAddress)
            {
                svcs.AddAuthorization();

                svcs.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = baseAddress;
                        options.RequireHttpsMetadata = false;
                        options.ApiName = TestIdentityServerConfiguration().Api.Name;
                    });
            }
        }
        protected virtual async Task ConfigureClientAuthentication(HttpClient client)
        {
            await ConfigureClientDiscoAuthentication(client).ConfigureAwait(false);
        }

        protected virtual async Task ConfigureClientDiscoAuthentication(HttpClient client)
        {
            var server = await Server.ConfigureAwait(false);
            var handler = server.CreateHandler();
            var authority = server.BaseAddress.ToString();
            var getUrl = authority + "/.well-known/openid-configuration";
            var get = await client.GetAsync(getUrl, CancellationToken).ConfigureAwait(false);
            var discoClient = new DiscoveryClient(authority, handler);
            var disco = await discoClient.GetAsync(CancellationToken).ConfigureAwait(false);
            if (!disco.IsError)
                await ConfigureClientDiscoAuthenticationFromTestClient(client, disco).ConfigureAwait(false);
        }

        private async Task ConfigureClientDiscoAuthenticationFromTestClient(HttpClient client, DiscoveryResponse disco)
        {
            var tc = TestIdentityServerConfiguration.Default.TestClient;
            var tokenClient = new TokenClient(disco.TokenEndpoint, tc.ClientId, tc.ClientSecrets.First().Value);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(tc.AllowedScopes.Single()).ConfigureAwait(false);
            client.SetBearerToken(tokenResponse.AccessToken);
        }
        protected override async Task<HttpClient> MakeClient()
        {
            var client = await base.MakeClient().ConfigureAwait(false);
            await ConfigureClientAuthentication(client).ConfigureAwait(false);
            return client;
        }
    }
    public class TestIdentityServerConfiguration
    {
        public static TestIdentityServerConfiguration Default = new TestIdentityServerConfiguration();
        public TestIdentityServerConfiguration() { }
        public ApiResource Api { get; } = new ApiResource("api", "Api");

        public IEnumerable<ApiResource> ApiResources() => new[] { Api };

        private Random Random { get; } = new Random();

        private Client _testClient;
        protected virtual Client MakeTestClient() => new Client {
            ClientId = "TestClient",
            AllowedGrantTypes = { GrantType.ClientCredentials },
            ClientSecrets =
                    {
                        new Secret($"secret".Sha256())
                    },
            AllowedScopes = { Api.Name }
        };
        public virtual Client TestClient => _testClient ?? (_testClient = MakeTestClient());

        public IEnumerable<Client> Clients() => new[] { TestClient };

        public static ICollection<IdentityResource> IdentityResources() => new[] { new IdentityResources.OpenId() };

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


    public class TestSourceBasedTests<TStartup> : DataAppTests<TStartup>
        where TStartup: class
    {
        private TestSource _testSource;

        public TestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory)
        {
            AppUnitOfWorkFactory = appUnitOfWorkFactory;
        }

        public IAppUnitOfWorkFactory AppUnitOfWorkFactory { get; }
        protected TestSource TestSource => _testSource ?? (_testSource = MakeTestSource());
        protected JsonSerializer JsonSerializer { get; } = new JsonSerializer();
        protected virtual TestSource MakeTestSource() => new TestSource(AppUnitOfWorkFactory);

        protected override void PreConfigureTestServices(IServiceCollection svcs)
        {
            svcs.Replace(ServiceDescriptor.Scoped(sp => AppUnitOfWorkFactory.UnitOfWork()));
            base.PreConfigureTestServices(svcs);
        }


    }
}