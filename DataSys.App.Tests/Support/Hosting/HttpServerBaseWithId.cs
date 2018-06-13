using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataSys.App.Tests.Support.Hosting;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DataSys.App.Tests.Support
{
    public abstract class HttpServerBaseWithId<TStartup> : HttpServerBase<TStartup>
        where TStartup : class
    {
        #region Identity4Server -- Server Setup
        protected abstract IIdentity4ServerConfiguration Identity4ServerConfiguration { get; }

        protected virtual IWebHostBuilder MakeIdBuilder() => new WebHostBuilder();
        protected virtual IWebHostBuilder ConfigureIdBuilder(IWebHostBuilder builder)
        {
            return builder
                .ConfigureServices(ConfigureIdentityServer)
                .Configure(app => app.UseIdentityServer());
        }
        protected virtual void ConfigureIdentityServer(IServiceCollection svcs)
        {
            Identity4ServerConfiguration.ConfigureServices(svcs);
        }
        private TestServer _idServer;
        protected TestServer IdServer => _idServer ?? (_idServer = new TestServer(ConfigureIdBuilder(MakeIdBuilder())));
        #endregion

        #region Regular server -- Setup Authentication
        protected override IWebHostBuilder MakeBuilder() => base.MakeBuilder().Configure(app => app.UseAuthentication());
        protected override void ConfigureTestServices(IServiceCollection svcs)
            => ConfigureIdentityServerAuthenticationOptions(svcs);
        protected virtual void ConfigureIdentityServerAuthenticationOptions(IServiceCollection svcs)
            => svcs.AddSingleton(IdentityServerAuthenticationOptions());
        protected virtual IdentityServerAuthenticationOptions IdentityServerAuthenticationOptions()
            => new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5001",
                RequireHttpsMetadata = false,
                ApiName = Identity4ServerConfiguration.Id4Clients.First().AllowedScopes.First(),

                // Traffic to the IdServer should go through handlers to that in-mem server
                JwtBackChannelHandler = IdServer.CreateHandler(),
                IntrospectionDiscoveryHandler = IdServer.CreateHandler(),
                IntrospectionBackChannelHandler = IdServer.CreateHandler()
            };
        #endregion

        #region OpenConnect Client setup (Using Identity4Server -- Client)
        protected virtual async Task ConfigureClientAuthentication(HttpClient client, CancellationToken cancellationToken)
            => await ConfigureClientDiscoAuthentication(client, cancellationToken).ConfigureAwait(false);

        protected virtual async Task ConfigureClientDiscoAuthentication(HttpClient client, CancellationToken cancellationToken)
        {
            var srv = IdServer;
            var handler = srv.CreateHandler();
            var authority = srv.BaseAddress.ToString();
            //var getUrl = authority + "/.well-known/openid-configuration";
            //var get = await client.GetAsync(getUrl, CancellationToken).ConfigureAwait(false);
            var discoClient = new DiscoveryClient(authority, handler);
            var idClient = Identity4ServerConfiguration.Id4Clients.First();
            var disco = await discoClient.GetAsync(cancellationToken).ConfigureAwait(false);
            if (!disco.IsError)
                await ConfigureClientDiscoAuthenticationFromTestClient(disco, idClient.ClientId, idClient.ClientSecrets.First().Value, idClient.AllowedScopes.First(), cancellationToken).ConfigureAwait(false);
        }

        public class ClientDiscoAuthenticationFailed: Exception
        {
            public TokenResponse Response { get; }
            public ClientDiscoAuthenticationFailed(TokenResponse tokenResponse)
                : base($"Client authentication failed: {tokenResponse.Error} {tokenResponse.ErrorDescription}\n{tokenResponse.Raw}")
            {
                Response = tokenResponse;
            }
        }
        private async Task ConfigureClientDiscoAuthenticationFromTestClient(DiscoveryResponse disco, string clientId, string secret, string scope, CancellationToken cancellationToken)
        {
            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, secret, IdServer.CreateHandler());
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(scope, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!tokenResponse.IsError)
                (await Client.ConfigureAwait(false)).SetBearerToken(tokenResponse.AccessToken);
        }
        #endregion

        #region Ensure (normal, Http) client is properly authenticated
        protected override async Task<HttpClient> MakeClient(CancellationToken cancellationToken)
        {
            var client = await base.MakeClient(cancellationToken).ConfigureAwait(false);
            await ConfigureClientAuthentication(client, cancellationToken).ConfigureAwait(false);
            return client;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            try
            {
                _idServer?.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }



    }
}