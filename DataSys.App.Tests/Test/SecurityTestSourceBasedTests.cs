using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataSys.App.DataAccess;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DataSys.App.Tests.Test
{
    public class SecurityTestSourceBasedTests : TestSourceBasedTests<SecurityTestSourceBasedTests.TestStartup>
    {
        public SecurityTestSourceBasedTests(IAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
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
            //var getUrl = authority + "/.well-known/openid-configuration";
            //await Task.Delay(TimeSpan.FromSeconds(5)); // TODO: REMOVE DEBUG
            //var get = await client.GetAsync(getUrl, CancellationToken).ConfigureAwait(false);
            var discoClient = new DiscoveryClient(authority, handler);
            var disco = await discoClient.GetAsync(CancellationToken).ConfigureAwait(false);
            if (!disco.IsError)
                await ConfigureClientDiscoAuthenticationFromTestClient(client, disco).ConfigureAwait(false);
        }

        private async Task ConfigureClientDiscoAuthenticationFromTestClient(HttpClient client, DiscoveryResponse disco)
        {
            var tc = TestIdentityServerConfiguration.Default.TestClient;
            var tokenClient = new TokenClient(disco.TokenEndpoint, tc.ClientId, tc.ClientSecrets.First().Value);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(tc.AllowedScopes.Single())
                .ConfigureAwait(false);
            if (tokenResponse.IsError)
                throw new Exception(
                    $"Client authentication failed: {tokenResponse.Error} {tokenResponse.ErrorDescription}\n{tokenResponse.Raw}");
            client.SetBearerToken(tokenResponse.AccessToken);
        }

        protected override async Task<HttpClient> MakeClient()
        {
            var client = await base.MakeClient().ConfigureAwait(false);
            await ConfigureClientAuthentication(client).ConfigureAwait(false);
            return client;
        }

        public class TestStartup : TestSourceBasedTests.TestStartup
        {
            public virtual TestIdentityServerConfiguration TestIdentityServerConfiguration() =>
                Test.TestIdentityServerConfiguration.Default;

            public override void ConfigureServices(IServiceCollection services)
            {
                SetupTrustedAuthentication(services, "http://localhost:5000/");
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
    }
}