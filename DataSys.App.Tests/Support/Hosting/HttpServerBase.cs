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
        private Task<HttpClient> _client;
        private TestServer _server;
        protected virtual CancellationToken CancellationToken => default(CancellationToken);
        protected TestServer Server => _server ?? (_server = new TestServer(ConfigureBuilder(MakeBuilder())));

        protected Task<HttpClient> Client
        {
            get => _client ?? (_client = MakeClient(CancellationToken));
            set => _client = value;
        }

        protected virtual IWebHostBuilder MakeBuilder() => new WebHostBuilder();

        protected virtual IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
            => builder.ConfigureTestServices(ConfigureTestServices);

        protected virtual void ConfigureTestServices(IServiceCollection svcs)
        {
        }

        protected virtual Task<HttpClient> MakeClient(CancellationToken cancellationToken) =>
            Task.FromResult(Server.CreateClient());

        protected override void Dispose(bool disposing)
        {
            try
            {
                try
                {
                    _client?.Dispose();
                }
                finally
                {
                    _server?.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}