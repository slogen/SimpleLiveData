using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSys.App.DataModel;
using DataSys.App.Presentation.SignalR;
using DataSys.App.Tests.Support;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Scm.DataAccess;
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

        public class ChangeData : IChange<Data>
        {
            public Data Entity { get; set; }

            public EntityChange Change { get; set; }

            object IChange.Entity => Entity;
        }

        [Fact]
        public async Task ObservingThoughApiWorks()
        {
            await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);
            var server = await Server.ConfigureAwait(false);
            var builder = new HubConnectionBuilder()
                .WithUrl($"http://test{LiveHub.Route}")
                .WithMessageHandler(_ => server.CreateHandler())
                ;
            var hubConnection = builder.Build();
            await hubConnection.StartAsync();
            var obs = hubConnection.Observe<ChangeData>(nameof(LiveHub.Observe), new object[] {null});
            var takeCount = 10;
            var sem = new SemaphoreSlim(0); // Coordinate progress between produced data and listener

            var obsTask = obs
                .Take(takeCount)
                .Select((v, idx) => new { v, idx })
                // Start a new producer whenever we have received 5 items
                .Do(x => { if (x.idx % 5 == 0) sem.Release(); })
                .Select(x => x.v)
                .ToListAsync(CancellationToken);

            var pushTask = TestSource.ProduceData(
                // Wait for sem for each batch of data
                    sem.ObserveRelease().Take(2).Select(_ => DateTime.UtcNow))
                .SelectMany(x => x)
                .ToListAsync(CancellationToken);

            // All ready, start pusing
            sem.Release();

            var observedChanges = await obsTask.ConfigureAwait(false);
            var observed = observedChanges.Select(c => c.Entity);
            var pushed = await pushTask.ConfigureAwait(false);

            var expect = pushed.Take(takeCount);
            observed.Should().BeEquivalentTo(expect,
                cfg => cfg.Excluding(x => x.Installation).Excluding(x => x.Signal));
            // Reception should contained interleaved data
            observed.Select(x => x.InstallationId)
                .Should().NotBeAscendingInOrder()
                .And.NotBeDescendingInOrder();
        }
    }

}