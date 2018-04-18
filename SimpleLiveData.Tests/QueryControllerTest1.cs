using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace SimpleLiveData.Tests
{
    [Collection("HttpApp")] // Share fully configured app between tests
    public class QueryControllerTest1
    {
        private readonly HttpApp _app;
        public QueryControllerTest1(HttpApp app)
        {
            _app = app;
        }
        protected CancellationToken CancellationToken => CancellationToken.None;
        [Fact]
        public async Task GetApplied()
        {
            var task = _app.Client.TestGetAsync($"/odata/Installation?$filter=contains(Name, '1')");
            //_app.Scheduler.Start();
            var str = await task.ConfigureAwait(false);
            str.Should().Be("foo");
        }
    }
}