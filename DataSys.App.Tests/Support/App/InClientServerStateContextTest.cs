using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataSys.App.Tests.Support.App.Source;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace DataSys.App.Tests.Test
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for tests that are run in a full context
    /// </summary>
    /// <remarks>Mainly provides easy access to context</remarks>
    /// <typeparam name="TContext">Specific type of context the test runs in</typeparam>
    public class InClientServerStateContextTest<TContext>: IClientServerStateContext
        where TContext: IClientServerStateContext
    {
        public TContext Context { get; }
        public CancellationToken CancellationToken => default(CancellationToken);

        public InClientServerStateContextTest(TContext context)
        {
            Context = context;
        }

        public void Dispose()
        {
            // The IClientServerStateContext is injected and under xUnit lifetime control
        }
        public Task<HttpClient> Client => Context.Client;
        public TestServer Server => Context.Server;
        public JsonSerializer JsonSerializer => Context.JsonSerializer;
        public TestSource TestSource => Context.TestSource;
        public Task Prepared => Context.Prepared;
    }
}