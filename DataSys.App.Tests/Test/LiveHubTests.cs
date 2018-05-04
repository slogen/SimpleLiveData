using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DataSys.App.DataModel;
using DataSys.App.Tests.Support;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Scm.Rx;
using Scm.Web;
using Xunit;

namespace DataSys.App.Tests.Test
{
    public class LiveHubTests : TestSourceBasedTests, IClassFixture<TestAppUnitOfWorkFactory>
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Specific type required for IoC")]
        public LiveHubTests(TestAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        [Fact]
        public async Task ObservingThoughApiWorks()
        {
            await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);
            var builder = new HubConnectionBuilder()
                .WithUrl("http://test/signalr/livedata")
                .WithMessageHandler(_ => Server.CreateHandler())
                .WithConsoleLogger()
                .WithTransport(TransportType.LongPolling);
            var hubConnection = builder.Build();
            await hubConnection.StartAsync();
            var obs = hubConnection.Observe<Data>("Observe", new object[] {null});
            var obsTask = obs
                .Do(x => Debug.WriteLine(x))
                .Take(10)
                .ToListAsync(CancellationToken);

            var incomingTask = TestSource.ProduceData(
                    Observable.Range(0, 2).Select(_ => DateTime.UtcNow))
                .SelectMany(x => x)
                .Take(10)
                .ToListAsync(CancellationToken);
            // Wait for that to actuall be started
            while (obsTask.Status == TaskStatus.WaitingForActivation || obsTask.Status == TaskStatus.WaitingToRun)
                await Task.Delay(TimeSpan.FromMilliseconds(1000), CancellationToken).ConfigureAwait(false);

            var results = await obsTask.ConfigureAwait(false);
            var incoming = await incomingTask.ConfigureAwait(false);
            results.Should().BeEquivalentTo(incoming,
                cfg => cfg.Excluding(x => x.Installation).Excluding(x => x.Signal));
            // TODO: Test the timing
        }
    }
}