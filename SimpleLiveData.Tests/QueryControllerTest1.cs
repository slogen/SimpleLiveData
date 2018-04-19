using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace SimpleLiveData.Tests
{
    public class QueryControllerTest1 : IDisposable
    {
        public QueryControllerTest1()
        {
            _app = new HttpApp();
        }

        public void Dispose()
        {
            _app?.Dispose();
        }

        private readonly HttpApp _app;
        protected TestSource TestSource => _app.TestSource;
        protected CancellationToken CancellationToken => CancellationToken.None;

        [Fact]
        public async Task GetApplied()
        {
            TestSource.Prepare(3, 3);
            TestSource.ProduceData(DateTime.UtcNow);
            var task = _app.Client.TestGetAsync(
                // "/odata/Installation?$expand=Data&$filter=contains(name, '1')"
                // $"/odata/Installation({_app.TestSource.Installations.Query(x => x.First()).Id})" // -- BROKEN!
                "/odata/Installation?$select=Name"
            );
            //_app.Scheduler.Start();
            var str = await task.ConfigureAwait(false);
            str.Should().Be("foo");
        }
    }
}