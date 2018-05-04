using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DataSys.App.Tests.Support;
using DataSys.Protocol;
using FluentAssertions;
using Scm.Sys;
using Scm.Web;
using Xunit;

namespace DataSys.App.Tests.Test
{
    public class PlainTests : TestSourceBasedTests, IClassFixture<TestAppUnitOfWorkFactory>
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Specific type required for IoC")]
        public PlainTests(TestAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        [Fact]
        public async Task GetById()
        {
            await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);
            var i1 = await TestSource.ObserveInstallations(ins => ins.ToObservable().FirstOrDefaultAsync());
            var task = Client.GetJsonAsync(
                new Uri(Server.BaseAddress, $"/api/Installation/{i1.Id}"),
                JsonSerializer
            );
            var result = await task.ConfigureAwait(false);
            var got = await result.Convert<Installation>(CancellationToken).ConfigureAwait(false);
            got.Should().BeEquivalentTo(new {i1.Id, i1.Name, i1.InstallationPeriod.From, i1.InstallationPeriod.To});
        }
    }
}