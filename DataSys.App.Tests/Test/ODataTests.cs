using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DataSys.App.Tests.Support.App;
using DataSys.Protocol;
using FluentAssertions;
using Microsoft.OData;
using Scm.Web;
using Xunit;

namespace DataSys.App.Tests.Test
{
    public class ODataTests : TestSourceBasedTests, IClassFixture<TestAppUnitOfWorkFactory>
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Specific type required for IoC")]
        public ODataTests(TestAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        protected async Task<IList<Installation>> QueryInstallations(string args)
        {
            return await Client.GetJsonAsync<List<Installation>>(
                new Uri(Server.BaseAddress, $"/odata/Installation?${args}"),
                JsonSerializer,
                CancellationToken
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task ODataNameOfInstallation()
        {
            await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);
            var lst = await QueryInstallations("select=Name,Id").ConfigureAwait(false);
            lst.Should().BeEquivalentTo(TestSource.Installations(), cfg => cfg.ExcludingMissingMembers());
        }
        [Fact]
        public async Task ODataOrderingNonExistingPropertiesThrows()
        {
            await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);
            0.Awaiting(async _ => await QueryInstallations("select=Name,TotallyNotAProperty").ConfigureAwait(false))
                .Should().Throw<ODataException>();
        }
    }
}