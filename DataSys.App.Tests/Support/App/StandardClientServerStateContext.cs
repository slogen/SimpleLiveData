using System;
using System.Net.Http;
using System.Threading.Tasks;
using DataSys.App.Tests.Support.App;
using DataSys.App.Tests.Support.App.Source;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace DataSys.App.Tests.Test
{
    /// <inheritdoc />
    /// <summary>
    /// Setup of <see cref="T:DataSys.App.Tests.Test.StandardClientServerStateContext`1" /> that uses <see cref="T:DataSys.App.Hosting.Startup" />
    /// </summary>
    public abstract class StandardClientServerStateContext : StandardClientServerStateContext<StandardClientServerStateContext.Startup>
    {
        public class Startup : Hosting.Startup
        {

        }
    }

    /// <inheritdoc cref="DataAppTests{TStartup}" />
    /// <summary>
    /// Base-class used for creating specific test contexts.
    /// Uses <see cref="T:DataSys.App.Tests.Support.App.DataAppTests`1" /> implement configuration of Http server and client
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public abstract class StandardClientServerStateContext<TStartup>: DataAppTests<TStartup>, IClientServerStateContext
        where TStartup : class
    {
        protected StandardClientServerStateContext()
        {
            TestSource = new TestSource(new TestAppUnitOfWorkFactory());
            _prepared = new Lazy<Task>(Prepare);
        }

        Task<HttpClient> IClientServerStateContext.Client => Client;

        TestServer IClientServerStateContext.Server => Server;

        protected override void ConfigureTestServices(IServiceCollection svcs)
        {
            svcs.Replace(ServiceDescriptor.Scoped(sp => TestSource.UnitOfWork()));
            base.ConfigureTestServices(svcs);
        }
        public JsonSerializer JsonSerializer { get; } = new JsonSerializer();

        public TestSource TestSource { get; }

        protected abstract Task Prepare();

        private readonly Lazy<Task> _prepared;
        public Task Prepared => _prepared.Value;
    }
}