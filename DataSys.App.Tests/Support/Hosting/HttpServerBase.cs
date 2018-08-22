using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Scm.Sys;

namespace DataSys.App.Tests.Support.Hosting
{
    /// <summary>
    /// Base for easy incremental addition of services and configuration
    /// </summary>
    public abstract class HttpServerBase : MissingDisposeDetection
    {
        private readonly Lazy<Task<HttpClient>> _client;
        private readonly Lazy<TestServer> _server;

        protected HttpServerBase()
        {
            _client = new Lazy<Task<HttpClient>>(() => MakeClient(CancellationToken));
            _server = new Lazy<TestServer>(MakeServer);
        }
        protected virtual CancellationToken CancellationToken => default(CancellationToken);
        protected TestServer Server => _server.Value;

        protected Task<HttpClient> Client => _client.Value;

        protected virtual IWebHostBuilder MakeBuilder() => new WebHostBuilder();

        protected virtual IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
            => builder.ConfigureTestServices(ConfigureTestServices);

        protected virtual void ConfigureTestServices(IServiceCollection svcs)
        {
        }

        protected virtual TestServer MakeServer() => new TestServer(ConfigureBuilder(MakeBuilder()));

        protected virtual Task<HttpClient> MakeClient(CancellationToken cancellationToken) =>
            Task.FromResult(Server.CreateClient());

        protected override void Dispose(bool disposing)
        {
            try
            {
                try
                {
                    if ( _client.IsValueCreated )
                        _client.Value.Dispose();
                }
                finally
                {
                    if (_server.IsValueCreated)
                        _server.Value.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}