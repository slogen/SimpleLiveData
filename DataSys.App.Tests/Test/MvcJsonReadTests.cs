using System;
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
    [Collection(nameof(StandardClientServer33ReadOnly))]
    public class MvcJsonReadTests : InClientServerStateContextTest<StandardClientServer33ReadOnly>
    {
        public MvcJsonReadTests(StandardClientServer33ReadOnly context) : base(context)
        {
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetTurbinesByIdReturnsExpectedJson(int index)
        {
            await Prepared.ConfigureAwait(false);
            var client = await Client.ConfigureAwait(false);
            var i = TestSource.Installations.Skip(index).First();
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