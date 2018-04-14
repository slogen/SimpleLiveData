using System;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using SimpleLiveData.App.Hosting;
using Xunit;

namespace SimpleLiveData.Tests
{
    public abstract class AbstractHttpConfigurationFixture : IDisposable
    {
        public virtual IWebHostBuilder Builder =>
            _builder ?? (_builder = MakeBuilder());
        public virtual TestServer Server => _server ?? (_server = MakeServer());
        public virtual HttpClient Client => _client ?? (_client = MakeClient());
        public Guid Id { get; } = Guid.NewGuid();

        protected abstract IWebHostBuilder MakeBuilder();

        protected virtual TestServer MakeServer()
        {
            return new TestServer(Builder);
        }

        protected virtual HttpClient MakeClient()
            => Server.CreateClient();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Internal variables
        private IWebHostBuilder _builder;
        private TestServer _server;
        private HttpClient _client;

        private static long _undisposedCount = 0;
        private static readonly ISubject<Unit> _missingDispose = new Subject<Unit>();
        public static IObservable<Unit> MissingDispose => _missingDispose.AsObservable();
        private static void NotifyMissingDispose()
        {
            Interlocked.Increment(ref _undisposedCount);
            _missingDispose.OnNext(Unit.Default);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                NotifyMissingDispose();
            try
            {
                _client?.Dispose();
            }
            finally
            {
                _server?.Dispose();
            }
        }
        #endregion


        ~AbstractHttpConfigurationFixture()
        {
            Dispose(false);
        }
    }

    [CollectionDefinition("HttpApp")] // Dclare how to share a fully configured Http interface to App
    public class HttpApp : AbstractHttpConfigurationFixture, ICollectionFixture<HttpApp>
    {
        protected override IWebHostBuilder MakeBuilder()
        => new WebHostBuilder().UseStartup<Startup>();
    }

    public static class TestExtensions
    {
        public static async Task<string> TestGetAsync(this HttpClient client, string uri)
        {
            var resp = await client.GetAsync(uri).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP Error: {resp.StatusCode}\n{await ExtractExplanation(resp)}");
            }

            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private static async Task<string> ExtractExplanation(HttpResponseMessage resp)
        {
            return await resp.Content.ReadAsStringAsync();
        }
    }

    [Collection("HttpApp")] // Share fully configured app between tests
    public class QueryControllerTest1
    {
        private readonly HttpApp _app;
        public QueryControllerTest1(HttpApp app)
        {
            this._app = app;
        }
        protected CancellationToken CancellationToken => CancellationToken.None;
        [Fact]
        public async Task GetApplied()
        {
            var str = await _app.Client.TestGetAsync("api/MyEntity/A").ConfigureAwait(false);
            str.Should().Be("foo");
        }
    }
}