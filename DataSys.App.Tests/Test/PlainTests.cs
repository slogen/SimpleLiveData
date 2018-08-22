using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DataSys.App.Presentation.Mvc;
using DataSys.App.Tests.Support.App;
using DataSys.Protocol;
using FluentAssertions;
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
        public async Task GetTurbinesByIdReturnsExpectedJson()
        {
            await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);
            var client = await Client.ConfigureAwait(false);
            var i = TestSource.Installations.Last();
            var got = await client.GetJsonAsync<Installation>(
                new Uri(Server.BaseAddress, $"{InstallationXController.RoutePrefix}/{i.Id}"),
                JsonSerializer,
                CancellationToken
            );
            var expected = new Installation(i.Id, i.Name, i.InstallationPeriod.From, i.InstallationPeriod.To);
            got.Should().BeEquivalentTo(expected);
        }
    }
}