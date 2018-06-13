using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DataSys.App.Tests.Support
{
    /// <summary>
    /// Base for easy incremental addition of services and configuration
    /// </summary>
    public abstract class HttpServerBase: MissingDisposeDetection
    {
        protected virtual CancellationToken CancellationToken => default(CancellationToken);

        protected virtual IWebHostBuilder MakeBuilder() => new WebHostBuilder();
        protected virtual IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder)
            => builder.ConfigureTestServices(ConfigureTestServices);
        protected virtual void ConfigureTestServices(IServiceCollection svcs)
        {
        }
        private TestServer _server;
        protected TestServer Server => _server ?? (_server = new TestServer(ConfigureBuilder(MakeBuilder())));

        private Task<HttpClient> _client;
        protected Task<HttpClient> Client
        {
            get => _client ?? (_client = MakeClient(CancellationToken));
            set => _client = value;
        }
        protected virtual Task<HttpClient> MakeClient(CancellationToken cancellationToken) => Task.FromResult(Server.CreateClient());

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