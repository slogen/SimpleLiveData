using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSys.App.Tests.Support.App;
using DataSys.Protocol;
using FluentAssertions;
using Microsoft.OData;
using Scm.Web;
using Xunit;

namespace DataSys.App.Tests.Test
{
    [Collection(nameof(StandardClientServer33ReadOnly))]
    public class ODataTests : InClientServerStateContextTest<StandardClientServer33ReadOnly>
    {
        public ODataTests(StandardClientServer33ReadOnly context): base(context)
        {
        }

        protected async Task<IList<Installation>> QueryInstallations(string args)
        {
            await Prepared.ConfigureAwait(false);
            return await Client.GetJsonAsync<List<Installation>>(
                new Uri(Server.BaseAddress, $"/odata/Installation?${args}"),
                JsonSerializer,
                CancellationToken
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task ODataNameOfInstallation()
        {
            await Prepared.ConfigureAwait(false);
            var lst = await QueryInstallations("select=Name,Id").ConfigureAwait(false);
            lst.Should().BeEquivalentTo(TestSource.Installations, cfg => cfg.ExcludingMissingMembers());
        }
        [Fact]
        public async Task ODataOrderingNonExistingPropertiesThrows()
        {
            await Prepared.ConfigureAwait(false);
            0.Awaiting(async _ => await QueryInstallations("select=Name,TotallyNotAProperty").ConfigureAwait(false))
                .Should().Throw<ODataException>();
        }
    }
}